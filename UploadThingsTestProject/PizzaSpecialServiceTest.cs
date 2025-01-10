using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;
using System.Text.Json;
using UploadThingsGrpcService.Application.Services;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Infrastructure;
using UploadThingsGrpcService.Infrastructure.Data;
using UploadThingsGrpcService.PizzaSpecialProto;

namespace UploadThingsTestProject
{

    public class PizzaSpecialServiceTest
    {
        // RESTful setup
        private JsonSerializerOptions? _jsonSerializerOptions;
        private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://localhost:7102/") };

        // gRPC setup
        private MSSQLContext? _MSSQLContext;
        private IConfiguration? _configuration;
        private PizzaSpecialServices? _pizzaSpecialService;
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

            // Initialize Unit of Work and PizzaSpecial Services
            _UnitofWorkRepository = new UnitofWork(_MSSQLContext);
            _pizzaSpecialService = new PizzaSpecialServices(_UnitofWorkRepository);
        }

        public class PizzaSpecialResponse
        {
            public List<PizzaSpecial>? PizzaSpecialData { get; set; }
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
        public async Task ReadByIDPizzaSpecial_FromRESTful()
        {
            // Make sure to check the data exist first
            HttpResponseMessage response = await _httpClient.GetAsync("v1/PizzaSpecial?Id=1&data_that_needed=Id,Name,Description,BasePrice,ImageUrl");
            response.IsSuccessStatusCode.Should().BeTrue();


            string jsonResponse = await response.Content.ReadAsStringAsync();
            PizzaSpecial? pizzaSpecialResponse = JsonSerializer.Deserialize<PizzaSpecial>(jsonResponse, _jsonSerializerOptions);

            Console.WriteLine("Tetoot");
            pizzaSpecialResponse.Should().NotBeNull();
            pizzaSpecialResponse?.Id.Should().NotBe(null);
        }

        [Test]
        public async Task ReadAllPizzaSpecial_FromRESTful()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/PizzaSpecial/GetAllList");
            response.IsSuccessStatusCode.Should().BeTrue();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            PizzaSpecialResponse? pizzaSpecialResponse = JsonSerializer.Deserialize<PizzaSpecialResponse>(jsonResponse, _jsonSerializerOptions);

            pizzaSpecialResponse.Should().NotBeNull();
            pizzaSpecialResponse?.PizzaSpecialData.Should().NotBeNull();
            pizzaSpecialResponse?.PizzaSpecialData.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task CreatePizzaSpecial_and_DeletePizzaSpecial_FromRESTful()
        {
            PizzaSpecial content = new()
            {
                Name = "ex esse cupidatat commodo",
                Description = "quis esse in",
                BasePrice = 1,
                ImageUrl = "Ut"
            };

            // Convert the object to JSON
            string jsonContent = JsonSerializer.Serialize(content);

            // Create the StringContent with JSON payload
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");


            HttpResponseMessage createResponse = await _httpClient.PostAsync("v1/PizzaSpecial", httpContent);
            createResponse.IsSuccessStatusCode.Should().BeTrue();

            string jsonResponse = await createResponse.Content.ReadAsStringAsync();
            PizzaSpecial? pizzaSpecialResponse = JsonSerializer.Deserialize<PizzaSpecial>(jsonResponse, _jsonSerializerOptions);

            pizzaSpecialResponse.Should().NotBeNull();

            HttpResponseMessage deleteResponse = await _httpClient.DeleteAsync($"v1/PizzaSpecial/{pizzaSpecialResponse?.Id}");
            deleteResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        [Test]
        public async Task UpdatePizzaSpecial_FromRESTful()
        {
            PizzaSpecial? content = new()
            {
                Id = 1,
                Name = "updateto ex esse cupidatat commodo",
                Description = "updateto quis esse in",
                BasePrice = 1,
                ImageUrl = "updateto Ut",
            };

            // Convert the object to JSON
            string jsonContent = JsonSerializer.Serialize(content);

            // Create the StringContent with JSON payload
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage updateResponse = await _httpClient.PutAsync("v1/PizzaSpecial", httpContent);
            updateResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        // gRpc Test
        [Test]
        public async Task ReadPizzaSpecialByID_ShouldReturnPizzaSpecialData()
        {
            // Arrange create pizzaSpecial
            int id = GetLatestIdAsync("PizzaSpecial");
            CreatePizzaSpecialRequest requestCreatePizzaSpecial = new() { Name = "test Read Create PizzaSpecial", Description = "test_type", BasePrice = 1.1234, ImageUrl = "Images/001" };

            // Arrange Read PizzaSpecial
            ReadPizzaSpecialRequest requestReadPizzaSpecial = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "Id", "Name", "Description", "BasePrice", "ImageUrl" } } };
            ReadPizzaSpecialResponse responseReadPizzaSpecialExpected = new() { Id = id, Name = "test Read Create PizzaSpecial", Description = "test_type", BasePrice = 1.1234, ImageUrl = "Images/001" };

            if (_pizzaSpecialService == null)
            {
                Assert.Fail("_pizzaSpecialService is null.");
            }
            else
            {
                // Act 1 Create PizzaSpecial
                await _pizzaSpecialService.CreatePizzaSpecial(requestCreatePizzaSpecial, It.IsAny<ServerCallContext>());

                // Act 2 Read PizzaSpecial
                ReadPizzaSpecialResponse responseReadPizzaSpecial = await _pizzaSpecialService.ReadPizzaSpecial(requestReadPizzaSpecial, It.IsAny<ServerCallContext>());

                // Assert
                Assert.That(responseReadPizzaSpecial, Is.EqualTo(responseReadPizzaSpecialExpected));

                // Delete created data to cleanup
                DeletePizzaSpecialResponse numberId = await _pizzaSpecialService.DeletePizzaSpecial(new DeletePizzaSpecialRequest() { Id = id }, It.IsAny<ServerCallContext>());

                // Assert Delete function
                Assert.That(id, Is.EqualTo(numberId.Id));
            }


        }

        [Test]
        public async Task ReadAllPizzaSpecial_ShouldReturnAllPizzaSpecialData()
        {

            if (_pizzaSpecialService == null)
            {
                Assert.Fail("_pizzaSpecialService is null.");
            }
            else
            {
                // Act
                GetAllResponse response = await _pizzaSpecialService.ListPizzaSpecial(new GetAllRequest(), It.IsAny<ServerCallContext>());

                // Assert
                Assert.That(response, Is.Not.Null);
            }
        }

        [Test]
        public async Task CreatePizzaSpecialandDeletePizzaSpecial_ShouldCreatePizzaSpecialtoDatabaseThenDeletePizzaSpecial()
        {
            // Arrange
            int id = GetLatestIdAsync("PizzaSpecial");
            CreatePizzaSpecialRequest requestCreatePizzaSpecial = new() { Name = "test Create PizzaSpecial Nunit", Description = "test_type", BasePrice = 1.1234, ImageUrl = "Images/001" };

            ReadPizzaSpecialRequest requestReadPizzaSpecial = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "Id", "Name", "Description", "BasePrice", "ImageUrl" } } }; // The Id will depend of the latest PizzaSpecial Data in the Database.
            ReadPizzaSpecialResponse responseExpected = new() { Id = id, Name = "test Create PizzaSpecial Nunit", Description = "test_type", BasePrice = 1.1234, ImageUrl = "Images/001" };

            if (_pizzaSpecialService == null)
            {
                Assert.Fail("_pizzaSpecialService is null.");
            }
            else
            {

                // Act 1 Create PizzaSpecial Then Read It
                await _pizzaSpecialService.CreatePizzaSpecial(requestCreatePizzaSpecial, It.IsAny<ServerCallContext>());
                ReadPizzaSpecialResponse responseReadPizzaSpecial = await _pizzaSpecialService.ReadPizzaSpecial(requestReadPizzaSpecial, It.IsAny<ServerCallContext>());
                // Assert Act 1, PizzaSpecial Should be in The Database
                Assert.That(responseReadPizzaSpecial, Is.EqualTo(responseExpected));

                // Act 2 Delete PizzaSpecial
                DeletePizzaSpecialResponse responseDelete = await _pizzaSpecialService.DeletePizzaSpecial(new DeletePizzaSpecialRequest { Id = id }, It.IsAny<ServerCallContext>());
                // Assert Act 2
                Assert.That(responseDelete, Is.EqualTo(new DeletePizzaSpecialResponse { Id = id }));
            }
        }

        [Test]
        public async Task UpdatePizzaSpecial_ShouldUpdatePizzaSpecialData()
        {
            // Arrange Update PizzaSpecial
            int id = 3;
            UpdatePizzaSpecialRequest requestUpdatePizzaSpecial = new() { Id = id, Name = "Update PizzaSpecial Test 1", Description = "test_type", BasePrice = 2, ImageUrl = "Images/002" };

            // Arrange Read PizzaSpecial
            ReadPizzaSpecialRequest requestReadPizzaSpecial = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "Id", "Name", "Description", "BasePrice", "ImageUrl" } } };
            ReadPizzaSpecialResponse responseExpected = new() { Id = id, Name = "Update PizzaSpecial Test 1", Description = "test_type", BasePrice = 2, ImageUrl = "Images/002" };

            if (_pizzaSpecialService == null)
            {
                Assert.Fail("_pizzaSpecialService is null.");
            }
            else
            {

                // Act Update PizzaSpecial
                await _pizzaSpecialService.UpdatePizzaSpecial(requestUpdatePizzaSpecial, It.IsAny<ServerCallContext>());
                // Act Read PizzaSpecial
                ReadPizzaSpecialResponse responseReadPizzaSpecial = await _pizzaSpecialService.ReadPizzaSpecial(requestReadPizzaSpecial, It.IsAny<ServerCallContext>());

                // Assert
                Assert.That(responseReadPizzaSpecial, Is.EqualTo(responseExpected));
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
