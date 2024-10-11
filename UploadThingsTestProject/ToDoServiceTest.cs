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
using UploadThingsGrpcService.ToDoProto;

namespace UploadThingsTestProject
{
    [TestFixture]
    public class ToDoServiceTest
    {
        private UnitofWork _unitofWorkRepository;
        private IConfiguration _configuration;
        private MSSQLContext _MSSQLContext;
        private ToDoServices _toDoService;
        private HttpClient _httpClient;
        private JsonSerializerOptions _jsonSerializerOptions;


        [SetUp]
        public void Setup()
        {
            // RESTful setup
            _httpClient = new() { BaseAddress = new Uri("https://localhost:7102/") };
            _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

            // gRpc setup
            IConfigurationBuilder Builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            _configuration = Builder.Build();

            DbContextOptions<MSSQLContext> Option = new DbContextOptionsBuilder<MSSQLContext>()
                .UseSqlServer(_configuration.GetConnectionString("MSSQLToDoDatabaseConnection"))
                .Options;

            _MSSQLContext = new MSSQLContext(Option);
            _unitofWorkRepository = new UnitofWork(_MSSQLContext);
            _toDoService = new ToDoServices(_unitofWorkRepository);
        }

        public class ToDoResponse
        {
            public List<ToDoItem>? ToDoData { get; set; }
        }

        private int GetLatestIdAsync(string table)
        {
            // Get latest id
            int id = 0;

            id = _MSSQLContext.Set<CurrentIdentity>()
                    .FromSqlRaw("SELECT CAST(IDENT_CURRENT('" + table + "') AS int) AS Id")
                    .AsEnumerable()
                    .Select(p => p.Id)
                    .FirstOrDefault();
            return id + 1;
        }

        // RESTful Test
        [Test]
        public async Task ReadByIDToDo_FromRESTful()
        {
            // Make sure to check the data exist first
            HttpResponseMessage response = await _httpClient.GetAsync("v1/todo?id=3&data_that_needed=id,title,description,todostatus");
            response.IsSuccessStatusCode.Should().BeTrue();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            User? ToDoResponse = JsonSerializer.Deserialize<User>(jsonResponse, _jsonSerializerOptions);

            ToDoResponse.Should().NotBeNull();
            ToDoResponse?.Id.Should().NotBe(null);
        }

        [Test]
        public async Task ReadAllToDo_FromRESTful()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("v1/todo/GetAllList");
            response.IsSuccessStatusCode.Should().BeTrue();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            ToDoResponse? ToDoResponse = JsonSerializer.Deserialize<ToDoResponse>(jsonResponse, _jsonSerializerOptions);

            ToDoResponse.Should().NotBeNull();
            ToDoResponse?.ToDoData.Should().NotBeNull();
            ToDoResponse?.ToDoData.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task CreateToDo_and_DeleteToDo_FromRESTful()
        {
            ToDoItem content = new()
            {
                Title = "Ut",
                Description = "ex esse cupidatat commodo"
            };

            HttpResponseMessage createResponse = await _httpClient.PostAsJsonAsync("v1/todo", content);
            createResponse.IsSuccessStatusCode.Should().BeTrue();

            string jsonResponse = await createResponse.Content.ReadAsStringAsync();
            ToDoItem? ToDoResponse = JsonSerializer.Deserialize<ToDoItem>(jsonResponse, _jsonSerializerOptions);

            ToDoResponse.Should().NotBeNull();

            HttpResponseMessage deleteResponse = await _httpClient.DeleteAsync($"v1/todo/{ToDoResponse?.Id}");
            deleteResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        [Test]
        public async Task UpdateToDo_FromRESTful()
        {
            ToDoItem? content = new()
            {
                Id = 10,
                Title = "updateto Ut",
                Description = "updateto ex esse cupidatat commodo",
                ToDoStatus = "UPDATETO",
            };

            HttpResponseMessage updateResponse = await _httpClient.PutAsJsonAsync("v1/todo", content);
            updateResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        // gRpc Test
        [Test]
        public async Task ReadToDoByID_ShouldReturnToDoData()
        {
            // Arrange
            ReadToDoRequest request = new() { Id = 1, DataThatNeeded = new FieldMask { Paths = { "id", "title", "description", "todostatus" } } };
            ReadToDoResponse responseExpected = new() { Id = 1, Title = "amet officia Excepteur", Description = "ipsum@gmail.com", ToDoStatus = "" };

            // Act
            var response = await _toDoService.ReadToDo(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(response, Is.EqualTo(responseExpected));
        }

        [Test]
        public async Task ReadAllToDo_ShouldReturnAllToDoData()
        {
            // Act
            GetAllResponse response = await _toDoService.ListToDo(new GetAllRequest(), It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task CreateToDoandDeleteToDo_ShouldCreateToDotoDatabaseThenDeleteToDo()
        {
            // Arrange
            int id = GetLatestIdAsync("ToDoItems");
            CreateTodoRequest requestCreateToDo = new() { Title = "Create ToDo Test", Description = "Create ToDo Succeed", };

            ReadToDoRequest requestReadToDo = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "title", "description", "todostatus" } } };
            ReadToDoResponse responseExpected = new() { Id = id, Title = "Create ToDo Test", Description = "Create ToDo Succeed", ToDoStatus = "NEW" };

            // Act 1 Create ToDo Then Read It
            await _toDoService.CreateToDo(requestCreateToDo, It.IsAny<ServerCallContext>());
            ReadToDoResponse responseReadToDo = await _toDoService.ReadToDo(requestReadToDo, It.IsAny<ServerCallContext>());
            // Assert Act 1, ToDo Should be in The Database
            Assert.That(responseReadToDo, Is.EqualTo(responseExpected));


            // Act 2 Delete ToDo
            DeleteToDoResponse responseDelete = await _toDoService.DeleteToDo(new DeleteToDoRequest { Id = id }, It.IsAny<ServerCallContext>());
            // Assert Act 2
            Assert.That(responseDelete, Is.EqualTo(new DeleteToDoResponse { Id = id }));

        }

        [Test]
        public async Task UpdateToDo_ShouldUpdateToDoData()
        {
            // Arrange Update ToDo
            UpdateToDoRequest requestUpdateToDo = new() { Id = 1, Title = "amet officia Excepteur", Description = "ipsum@gmail.com" };
            // Arrange Read ToDo
            ReadToDoRequest requestReadToDo = new() { Id = 1, DataThatNeeded = new FieldMask { Paths = { "id", "title", "description" } } };
            ReadToDoResponse responseExpected = new() { Id = 1, Title = "amet officia Excepteur", Description = "ipsum@gmail.com" };

            // Act Update ToDo
            await _toDoService.UpdateToDo(requestUpdateToDo, It.IsAny<ServerCallContext>());
            // Act Read ToDo
            ReadToDoResponse responseReadToDo = await _toDoService.ReadToDo(requestReadToDo, It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(responseReadToDo, Is.EqualTo(responseExpected));

        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            _MSSQLContext.Dispose();
            _unitofWorkRepository.Dispose();
            _httpClient.Dispose();
        }
    }
}
