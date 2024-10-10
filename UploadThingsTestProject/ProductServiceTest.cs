using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using UploadThingsGrpcService.Application.Services;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Infrastructure;
using UploadThingsGrpcService.Infrastructure.Data;
using UploadThingsGrpcService.ProductProto;
namespace UploadThingsTestProject
{

    class ProductServiceTest
    {
        private UnitofWork _UnitofWorkRepository;
        private IConfiguration _configuration;
        private MSSQLContext _MSSQLContext;
        private ProductServices _productService;

        [SetUp]
        public void Setup()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            _configuration = configurationBuilder.Build();

            var option = new DbContextOptionsBuilder<MSSQLContext>()
                .UseSqlServer(_configuration.GetConnectionString("MSSQLToDoDatabaseConnection"))
                .Options;

            _MSSQLContext = new MSSQLContext(option);
            _UnitofWorkRepository = new UnitofWork(_MSSQLContext);
            _productService = new ProductServices(_UnitofWorkRepository);
        }

        private decimal GetLatestIdAsync(string table)
        {
            // Get latest id
            decimal id = 0;

            id = _MSSQLContext.Set<CurrentIdentity>()
                    .FromSqlRaw("SELECT IDENT_CURRENT('" + table + "') AS Id")
                    .AsEnumerable()
                    .Select(p => p.Id)
                    .FirstOrDefault();
            return id + 1;
        }

        [Test]
        public async Task ReadProductByID_ShouldReturnProductData()
        {
            // Arrange create product
            int id = (int)GetLatestIdAsync("Product");
            CreateProductRequest requestCreateProduct = new() { ProductName = "test Read Create Product", ProductType = "test_type", ProductPrice = 1.1234, ProductImagePath = "Images/001" };

            // Arrange Read Product
            ReadProductRequest requestReadProduct = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "productname", "producttype", "productprice", "productimagepath" } } };
            ReadProductResponse responseReadProductExpected = new() { Id = id, ProductName = "test Read Create Product", ProductType = "test_type", ProductPrice = 1.1234, ProductImagePath = "Images/001" };

            // Act 1 Create Product
            await _productService.CreateProduct(requestCreateProduct, It.IsAny<ServerCallContext>());
            // Act 2 Read Product
            ReadProductResponse responseReadProduct = await _productService.ReadProduct(requestReadProduct, It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(responseReadProduct, Is.EqualTo(responseReadProductExpected));

            // Delete created data to cleanup
            DeleteProductResponse numberId = await _productService.DeleteProduct(new DeleteProductRequest() { Id = id }, It.IsAny<ServerCallContext>());

            // Assert Delete function
            Assert.That(id, Is.EqualTo(numberId.Id));
        }

        [Test]
        public async Task ReadAllProduct_ShouldReturnAllProductData()
        {
            // Act
            GetAllResponse response = await _productService.ListProduct(new GetAllRequest(), It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task CreateProductandDeleteProduct_ShouldCreateProducttoDatabaseThenDeleteProduct()
        {
            // Arrange
            int id = (int)GetLatestIdAsync("Product");
            CreateProductRequest requestCreateProduct = new() { ProductName = "test Create Product Nunit", ProductType = "test_type", ProductPrice = 1.1234, ProductImagePath = "Images/001" };

            ReadProductRequest requestReadProduct = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "productname", "producttype", "productprice", "productimagepath" } } }; // The Id will depend of the latest Product Data in the Database.
            ReadProductResponse responseExpected = new() { Id = id, ProductName = "test Create Product Nunit", ProductType = "test_type", ProductPrice = 1.1234, ProductImagePath = "Images/001" };

            // Act 1 Create Product Then Read It
            await _productService.CreateProduct(requestCreateProduct, It.IsAny<ServerCallContext>());
            ReadProductResponse responseReadProduct = await _productService.ReadProduct(requestReadProduct, It.IsAny<ServerCallContext>());
            // Assert Act 1, Product Should be in The Database
            Assert.That(responseReadProduct, Is.EqualTo(responseExpected));

            // Act 2 Delete Product
            DeleteProductResponse responseDelete = await _productService.DeleteProduct(new DeleteProductRequest { Id = id }, It.IsAny<ServerCallContext>());
            // Assert Act 2
            Assert.That(responseDelete, Is.EqualTo(new DeleteProductResponse { Id = id }));

        }

        [Test]
        public async Task UpdateProduct_ShouldUpdateProductData()
        {
            // Arrange Update Product
            int id = 2;
            UpdateProductRequest requestUpdateProduct = new() { Id = id, ProductName = "Update Product Test 1", ProductType = "test_type", ProductPrice = 2, ProductImagePath = "Images/002" };

            // Arrange Read Product
            ReadProductRequest requestReadProduct = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "productname", "producttype", "productprice", "productimagepath" } } };
            ReadProductResponse responseExpected = new() { Id = id, ProductName = "Update Product Test 1", ProductType = "test_type", ProductPrice = 2, ProductImagePath = "Images/002" };

            // Act Update Product
            await _productService.UpdateProduct(requestUpdateProduct, It.IsAny<ServerCallContext>());
            // Act Read Product
            ReadProductResponse responseReadProduct = await _productService.ReadProduct(requestReadProduct, It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(responseReadProduct, Is.EqualTo(responseExpected));

        }

        [TearDown]
        public void TearDown()
        {
            _MSSQLContext.Dispose();
            _UnitofWorkRepository.Dispose();
        }
    }
}
