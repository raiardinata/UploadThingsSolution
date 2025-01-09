using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net.Http.Json;
using System.Text.Json;
using UploadThingsGrpcService.Application.Services;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Infrastructure;
using UploadThingsGrpcService.Infrastructure.Data;
using UploadThingsGrpcService.ProductProto;
namespace UploadThingsTestProject
{

    public class ProductServiceTest
    {
        // RESTful setup
        private JsonSerializerOptions? _jsonSerializerOptions;
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:7102/") };

        // gRPC setup
        private MSSQLContext? _MSSQLContext;
        private IConfiguration? _configuration;
        private ProductServices? _productService;
        private UnitofWork? _UnitofWorkRepository;
        private DbContextOptions<MSSQLContext>? _options;

        [SetUp]
        public void Setup()
        {
            // Initialize RESTful client
            _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Initialize configuration
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            _configuration = configurationBuilder.Build();

            // Initialize DbContext options and MSSQL context
            _options = new DbContextOptionsBuilder<MSSQLContext>()
                .UseSqlServer(_configuration.GetConnectionString("MSSQLToDoDatabaseConnection"))
                .Options;
            _MSSQLContext = new MSSQLContext(_options);

            // Initialize Unit of Work and Product Services
            _UnitofWorkRepository = new UnitofWork(_MSSQLContext);
            _productService = new ProductServices(_UnitofWorkRepository);
        }

        public class ProductResponse
        {
            public List<Product>? ProductData { get; set; }
        }

        private int GetLatestIdAsync(string table)
        {
            // Get latest id
            int id = 0;

            id = _MSSQLContext?.Set<CurrentIdentity>()
                    .FromSqlRaw("SELECT CAST(IDENT_CURRENT('" + table + "') AS int) AS Id")
                    .AsEnumerable()
                    .Select(static p => p.Id)
                    .FirstOrDefault() ?? 0;
            return id + 1;
        }

        // RESTful Test
        [Test]
        public async Task ReadByIDProduct_FromRESTful()
        {
            // Make sure to check the data exist first
            HttpResponseMessage response = await _httpClient.GetAsync("v1/Product?id=3&data_that_needed=id,productname,producttype,productprice,productimagepath");
            response.IsSuccessStatusCode.Should().BeTrue();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            User? productResponse = JsonSerializer.Deserialize<User>(jsonResponse, _jsonSerializerOptions);

            productResponse.Should().NotBeNull();
            productResponse?.Id.Should().NotBe(null);
        }

        [Test]
        public async Task ReadAllProduct_FromRESTful()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/Product/GetAllList");
            response.IsSuccessStatusCode.Should().BeTrue();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            ProductResponse? productResponse = JsonSerializer.Deserialize<ProductResponse>(jsonResponse, _jsonSerializerOptions);

            productResponse.Should().NotBeNull();
            productResponse?.ProductData.Should().NotBeNull();
            productResponse?.ProductData.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task CreateProduct_and_DeleteProduct_FromRESTful()
        {
            Product content = new()
            {
                ProductImagePath = "Ut",
                ProductName = "ex esse cupidatat commodo",
                ProductPrice = 1,
                ProductType = "quis esse in"
            };

            HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("v1/Product", content);
            createResponse.IsSuccessStatusCode.Should().BeTrue();

            string jsonResponse = await createResponse.Content.ReadAsStringAsync();
            Product? productResponse = JsonSerializer.Deserialize<Product>(jsonResponse, _jsonSerializerOptions);

            productResponse.Should().NotBeNull();

            HttpResponseMessage deleteResponse = await _httpClient.DeleteAsync($"v1/Product/{productResponse?.Id}");
            deleteResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        [Test]
        public async Task UpdateProduct_FromRESTful()
        {
            Product? content = new()
            {
                Id = 47,
                ProductImagePath = "updateto Ut",
                ProductName = "updateto ex esse cupidatat commodo",
                ProductPrice = 1,
                ProductType = "updateto quis esse in"
            };

            HttpResponseMessage updateResponse = await _httpClient.PutAsJsonAsync("v1/Product", content);
            updateResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        // gRpc Test
        [Test]
        public async Task ReadProductByID_ShouldReturnProductData()
        {
            // Arrange create product
            int id = (int)GetLatestIdAsync("Product");
            CreateProductRequest requestCreateProduct = new() { ProductName = "test Read Create Product", ProductType = "test_type", ProductPrice = 1.1234, ProductImagePath = "Images/001" };

            // Arrange Read Product
            ReadProductRequest requestReadProduct = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "productname", "producttype", "productprice", "productimagepath" } } };
            ReadProductResponse responseReadProductExpected = new() { Id = id, ProductName = "test Read Create Product", ProductType = "test_type", ProductPrice = 1.1234, ProductImagePath = "Images/001" };

            if (_productService == null)
            {
                Assert.Fail("_productService is null.");
            }
            else
            {
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


        }

        [Test]
        public async Task ReadAllProduct_ShouldReturnAllProductData()
        {

            if (_productService == null)
            {
                Assert.Fail("_productService is null.");
            }
            else
            {
                // Act
                GetAllResponse response = await _productService.ListProduct(new GetAllRequest(), It.IsAny<ServerCallContext>());

                // Assert
                Assert.That(response, Is.Not.Null);
            }
        }

        [Test]
        public async Task CreateProductandDeleteProduct_ShouldCreateProducttoDatabaseThenDeleteProduct()
        {
            // Arrange
            int id = GetLatestIdAsync("Product");
            CreateProductRequest requestCreateProduct = new() { ProductName = "test Create Product Nunit", ProductType = "test_type", ProductPrice = 1.1234, ProductImagePath = "Images/001" };

            ReadProductRequest requestReadProduct = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "productname", "producttype", "productprice", "productimagepath" } } }; // The Id will depend of the latest Product Data in the Database.
            ReadProductResponse responseExpected = new() { Id = id, ProductName = "test Create Product Nunit", ProductType = "test_type", ProductPrice = 1.1234, ProductImagePath = "Images/001" };

            if (_productService == null)
            {
                Assert.Fail("_productService is null.");
            }
            else
            {

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
        }

        [Test]
        public async Task UpdateProduct_ShouldUpdateProductData()
        {
            // Arrange Update Product
            int id = 3;
            UpdateProductRequest requestUpdateProduct = new() { Id = id, ProductName = "Update Product Test 1", ProductType = "test_type", ProductPrice = 2, ProductImagePath = "Images/002" };

            // Arrange Read Product
            ReadProductRequest requestReadProduct = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "productname", "producttype", "productprice", "productimagepath" } } };
            ReadProductResponse responseExpected = new() { Id = id, ProductName = "Update Product Test 1", ProductType = "test_type", ProductPrice = 2, ProductImagePath = "Images/002" };

            if (_productService == null)
            {
                Assert.Fail("_productService is null.");
            }
            else
            {

                // Act Update Product
                await _productService.UpdateProduct(requestUpdateProduct, It.IsAny<ServerCallContext>());
                // Act Read Product
                ReadProductResponse responseReadProduct = await _productService.ReadProduct(requestReadProduct, It.IsAny<ServerCallContext>());

                // Assert
                Assert.That(responseReadProduct, Is.EqualTo(responseExpected));
            }
        }

        [TearDown]
        public void TearDown()
        {
            _MSSQLContext!.Dispose();
            _UnitofWorkRepository!.Dispose();
            _httpClient.Dispose();
        }
    }
}
