using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
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


        [SetUp]
        public void Setup()
        {
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
        public async Task ReadToDoByID_ShouldReturnToDoData()
        {
            // Arrange
            var request = new ReadToDoRequest { Id = 1, DataThatNeeded = new FieldMask { Paths = { "id", "title", "description", "todostatus" } } };
            var responseExpected = new ReadToDoResponse { Id = 1, Title = "amet officia Excepteur", Description = "ipsum@gmail.com", ToDoStatus = "" };

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
            int id = (int)GetLatestIdAsync("ToDoItems");
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
        }
    }
}
