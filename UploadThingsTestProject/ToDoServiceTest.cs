using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;
using UploadThingsGrpcService.Infrastructure.Data;
using UploadThingsGrpcService.Infrastructure.Repositories;
using UploadThingsGrpcService.Presentation.Services;
using UploadThingsGrpcService.ToDoProto;

namespace UploadThingsTestProject
{
    [TestFixture]
    public class ToDoServiceTest
    {
        private IToDoRepository _toDoRepository;
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
            _toDoRepository = new ToDoRepository(_MSSQLContext);
            _toDoService = new ToDoServices(_toDoRepository);
        }

        [Test]
        public async Task ReadToDoByID_ShouldReturnToDoData()
        {
            // Arrange
            var request = new ReadToDoRequest { Id = 1, DataThatNeeded = new FieldMask { Paths = { "id", "title", "description", "todostatus" } } };
            var responseExpected = new ReadToDoResponse { Id = 1, Title = "Create ToDo Test", Description = "Create ToDo Succeed", ToDoStatus = "NEW" };

            // Act
            var response = await _toDoService.ReadToDo(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(response, Is.EqualTo(responseExpected));
        }

        [Test]
        public async Task ReadAllToDo_ShouldReturnAllToDoData()
        {
            // Arrange
            GetAllRequest request = new();
            GetAllResponse responseExpected = new()
            {
                TodoData = {
                    new ReadToDoResponse { Id = 1, Title = "Rai Ardinata", Description = "raiardinata@gmail.com", ToDoStatus = "" },
                    new ReadToDoResponse { Id = 3, Title = "Abdul Somat", Description = "abdulsomat@gmail.com", ToDoStatus = "" },
                    new ReadToDoResponse { Id = 6, Title = "amet officia Excepteur", Description = "ipsum@gmail.com", ToDoStatus = "" },
                }
            };

            // Act
            GetAllResponse response = await _toDoService.ListToDo(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(response, Is.EqualTo(responseExpected));
        }

        [Test]
        public async Task CreateToDoandDeleteToDo_ShouldCreateToDotoDatabaseThenDeleteToDo()
        {
            int id = 0;
            // Get latest id
            ToDoItem toDoItemObject = await _MSSQLContext.Set<ToDoItem>().OrderByDescending(ToDoItems => ToDoItems.Id).FirstOrDefaultAsync() ?? new ToDoItem();
            if (toDoItemObject == null)
            {
                Assert.Fail("");
            }
            else
            {
                // C# cannot check we already make sure it's not null eventhough there is toDoItemObject == null, is there a workaround for this?
                id = (int)toDoItemObject.Id + 1;
            }

            // Arrange
            CreateTodoRequest requestCreateToDo = new() { Title = "Create ToDo Test", Description = "Create ToDo Succeed", };
            // The Id will depend of the latest ToDo Data in the Database.
            ReadToDoRequest requestReadToDo = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "title", "description", "todostatus" } } };
            ReadToDoResponse responseExpected = new() { Id = id, Title = "Create ToDo Test", Description = "Create ToDo Succeed", ToDoStatus = "NEW" };

            // Act 1 Create ToDo Then Read It
            CreateTodoResponse responseCreateToDo = await _toDoService.CreateToDo(requestCreateToDo, It.IsAny<ServerCallContext>());
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
            UpdateToDoRequest requestUpdateToDo = new() { Id = 6, Title = "amet officia Excepteur", Description = "ipsum@gmail.com" };
            // Arrange Read ToDo
            ReadToDoRequest requestReadToDo = new() { Id = 6, DataThatNeeded = new FieldMask { Paths = { "id", "title", "description" } } };
            ReadToDoResponse responseExpected = new() { Id = 6, Title = "amet officia Excepteur", Description = "ipsum@gmail.com" };

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
        }
    }
}
