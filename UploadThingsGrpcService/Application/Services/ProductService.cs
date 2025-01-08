using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.ProductProto;

namespace UploadThingsGrpcService.Application.Services
{
    public class ProductServices(IUnitOfWork unitofWorkRepository) : ProductService.ProductServiceBase
    {
        private readonly IUnitOfWork _unitofWorkRepository = unitofWorkRepository;

        public ReadProductResponse ApplyFieldMask(ReadProductResponse fullData, FieldMask fieldMask)
        {
            ReadProductResponse selectedData = new();
            foreach (string? field in fieldMask.Paths)
            {
                switch (field)
                {
                    case "id":
                        selectedData.Id = fullData.Id;
                        break;
                    case "productname":
                        selectedData.ProductName = fullData.ProductName;
                        break;
                    case "producttype":
                        selectedData.ProductType = fullData.ProductType;
                        break;
                    case "productimagepath":
                        selectedData.ProductImagePath = fullData.ProductImagePath;
                        break;
                    case "productprice":
                        selectedData.ProductPrice = fullData.ProductPrice;
                        break;
                    default:
                        break;
                }
            }
            return selectedData;
        }

        // Test to use RAFunction Product Repository method The other function is from General Repository
        public override async Task<TestResponse> RAFunction(TestRequest request, ServerCallContext context)
        {
            TestResponse test = new()
            {
                Hello = await _unitofWorkRepository.ProductRepository.Test()
            };
            return await Task.FromResult(test);
        }

        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            if (request.ProductName == string.Empty || request.ProductType == string.Empty || request.ProductImagePath == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            Product product = new()
            {
                ProductName = request.ProductName,
                ProductType = request.ProductType,
                ProductImagePath = request.ProductImagePath,
                ProductPrice = (decimal)request.ProductPrice
            };
            await _unitofWorkRepository.ProductRepository.AddAsync(product);
            return await Task.FromResult(new CreateProductResponse { Id = product.Id });
        }

        public override async Task<ReadProductResponse> ReadProduct(ReadProductRequest request, ServerCallContext context)
        {
            if (request.DataThatNeeded == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index does not contain data."));

            Product? productItem = await _unitofWorkRepository.ProductRepository.GetByIdAsync(request.Id);
            if (productItem != null)
            {
                ReadProductResponse readfulldata = new()
                {
                    Id = productItem.Id,
                    ProductName = productItem?.ProductName,
                    ProductImagePath = productItem?.ProductImagePath,
                    ProductType = productItem?.ProductType,
                    ProductPrice = (double)productItem?.ProductPrice!,
                };
                ReadProductResponse selectedData = ApplyFieldMask(readfulldata, request.DataThatNeeded);

                return await Task.FromResult(new ReadProductResponse
                {
                    Id = selectedData.Id,
                    ProductName = selectedData?.ProductName,
                    ProductImagePath = selectedData?.ProductImagePath,
                    ProductType = selectedData?.ProductType,
                    ProductPrice = selectedData?.ProductPrice ?? 0,
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListProduct(GetAllRequest request, ServerCallContext context)
        {
            GetAllResponse response = new();
            IEnumerable<Product> productItem = await _unitofWorkRepository.ProductRepository.GetAllAsync();
            foreach (Product? product in productItem)
            {
                response.ProductData.Add(new ReadProductResponse
                {
                    Id = product.Id,
                    ProductName = product?.ProductName,
                    ProductImagePath = product?.ProductImagePath,
                    ProductType = product?.ProductType,
                    ProductPrice = (double)product?.ProductPrice!,
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            if (request.ProductName == string.Empty || request.ProductType == string.Empty || request.ProductImagePath == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Some of the data might be empty."));

            Product productItem = await _unitofWorkRepository.ProductRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            productItem.ProductName = request.ProductName;
            productItem.ProductImagePath = request.ProductImagePath;
            productItem.ProductType = request.ProductType;
            productItem.ProductPrice = (decimal)request.ProductPrice;

            await _unitofWorkRepository.ProductRepository.UpdateAsync(productItem);
            return await Task.FromResult(new UpdateProductResponse { Id = request.Id });
        }

        public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            Product productItem = await _unitofWorkRepository.ProductRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            await _unitofWorkRepository.ProductRepository.DeleteAsync(productItem.Id);

            return await Task.FromResult(new DeleteProductResponse { Id = request.Id });
        }
    }
}
