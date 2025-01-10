using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.PizzaSpecialProto;

namespace UploadThingsGrpcService.Application.Services
{
    public class PizzaSpecialServices(IUnitOfWork unitofWorkRepository) : PizzaSpecialService.PizzaSpecialServiceBase
    {
        private readonly IUnitOfWork _unitofWorkRepository = unitofWorkRepository;

        public ReadPizzaSpecialResponse ApplyFieldMask(ReadPizzaSpecialResponse fullData, FieldMask fieldMask)
        {
            ReadPizzaSpecialResponse selectedData = new();
            foreach (string? field in fieldMask.Paths)
            {
                switch (field.ToLower())
                {
                    case "id":
                        selectedData.Id = fullData.Id;
                        break;
                    case "name":
                        selectedData.Name = fullData.Name;
                        break;
                    case "description":
                        selectedData.Description = fullData.Description;
                        break;
                    case "baseprice":
                        selectedData.BasePrice = fullData.BasePrice;
                        break;
                    case "imageurl":
                        selectedData.ImageUrl = fullData.ImageUrl;
                        break;
                    default:
                        break;
                }
            }
            return selectedData;
        }

        public override async Task<CreatePizzaSpecialResponse> CreatePizzaSpecial(CreatePizzaSpecialRequest request, ServerCallContext context)
        {
            if (request.Name == string.Empty || request.Description == string.Empty || request.ImageUrl == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            PizzaSpecial pizzaSpecial = new()
            {
                Name = request.Name,
                BasePrice = (decimal)request.BasePrice,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
            };
            await _unitofWorkRepository.PizzaSpecialRepository.AddAsync(pizzaSpecial);
            return await Task.FromResult(new CreatePizzaSpecialResponse { Id = pizzaSpecial.Id });
        }

        public override async Task<ReadPizzaSpecialResponse> ReadPizzaSpecial(ReadPizzaSpecialRequest request, ServerCallContext context)
        {
            if (request.DataThatNeeded == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Resource index does not contain data."));

            PizzaSpecial? pizzaSpecialItem = await _unitofWorkRepository.PizzaSpecialRepository.GetByIdAsync(request.Id);
            if (pizzaSpecialItem != null)
            {
                ReadPizzaSpecialResponse readfulldata = new()
                {
                    Id = pizzaSpecialItem.Id,
                    Name = pizzaSpecialItem?.Name,
                    Description = pizzaSpecialItem?.Description,
                    BasePrice = (double)pizzaSpecialItem!.BasePrice,
                    ImageUrl = pizzaSpecialItem?.ImageUrl,
                };
                ReadPizzaSpecialResponse selectedData = ApplyFieldMask(readfulldata, request.DataThatNeeded);

                return await Task.FromResult(new ReadPizzaSpecialResponse
                {
                    Id = selectedData.Id,
                    Name = selectedData?.Name,
                    Description = selectedData?.Description,
                    BasePrice = selectedData!.BasePrice,
                    ImageUrl = selectedData?.ImageUrl,
                });
            }

            throw new RpcException(new Status(StatusCode.NotFound, $"No task with id {request.Id}"));
        }

        public override async Task<GetAllResponse> ListPizzaSpecial(GetAllRequest request, ServerCallContext context)
        {
            GetAllResponse response = new();
            IEnumerable<PizzaSpecial> pizzaSpecialItem = await _unitofWorkRepository.PizzaSpecialRepository.GetAllAsync();
            foreach (PizzaSpecial? pizzaSpecial in pizzaSpecialItem)
            {
                response.PizzaSpecialData.Add(new ReadPizzaSpecialResponse
                {
                    Id = pizzaSpecial.Id,
                    Name = pizzaSpecial?.Name,
                    Description = pizzaSpecial?.Description,
                    BasePrice = (double)pizzaSpecial!.BasePrice,
                    ImageUrl = pizzaSpecial?.ImageUrl,
                });
            }
            return await Task.FromResult(response);
        }

        public override async Task<UpdatePizzaSpecialResponse> UpdatePizzaSpecial(UpdatePizzaSpecialRequest request, ServerCallContext context)
        {
            if (request.Name == string.Empty || request.Description == string.Empty)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Some of the data might be empty."));

            PizzaSpecial pizzaSpecialItem = await _unitofWorkRepository.PizzaSpecialRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            pizzaSpecialItem.Name = request.Name;
            pizzaSpecialItem.BasePrice = (decimal)request.BasePrice;
            pizzaSpecialItem.Description = request.Description;
            pizzaSpecialItem.ImageUrl = request.ImageUrl;

            await _unitofWorkRepository.PizzaSpecialRepository.UpdateAsync(pizzaSpecialItem);
            return await Task.FromResult(new UpdatePizzaSpecialResponse { Id = request.Id });
        }

        public override async Task<DeletePizzaSpecialResponse> DeletePizzaSpecial(DeletePizzaSpecialRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid supply of argument object."));

            PizzaSpecial pizzaSpecialItem = await _unitofWorkRepository.PizzaSpecialRepository.GetByIdAsync(request.Id) ?? throw new RpcException(new Status(StatusCode.InvalidArgument, $"No task with Id {request.Id}"));

            await _unitofWorkRepository.PizzaSpecialRepository.DeleteAsync(pizzaSpecialItem.Id);

            return await Task.FromResult(new DeletePizzaSpecialResponse { Id = request.Id });
        }
    }
}
