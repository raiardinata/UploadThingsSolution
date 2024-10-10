﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using UploadThingsGrpcService.Application.Services;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Infrastructure;
using UploadThingsGrpcService.Infrastructure.Data;
using UploadThingsGrpcService.UserProto;

namespace UploadThingsTestProject
{
    class UserServiceTest
    {
        private UnitofWork _unitofWorkRepository;
        private IConfiguration _configuration;
        private MSSQLContext _MSSQLContext;
        private UserServices _userServices;

        [SetUp]
        public void SetUp()
        {
            // Build configuration from appsettings.Development.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();

            // Set up real DB connection, make sure it development database
            var options = new DbContextOptionsBuilder<MSSQLContext>()
                .UseSqlServer(_configuration.GetConnectionString("MSSQLToDoDatabaseConnection"))
                .Options;

            _MSSQLContext = new MSSQLContext(options);
            _unitofWorkRepository = new UnitofWork(_MSSQLContext);
            _userServices = new UserServices(_unitofWorkRepository);
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
        public async Task ReadUserByID_ShouldReturnUserData()
        {
            // Arrange
            var request = new ReadUserRequest { Id = 6, DataThatNeeded = new FieldMask { Paths = { "id", "name", "email" } } };
            var responseExpected = new ReadUserResponse { Id = 6, Name = "amet officia Excepteur", Email = "ipsum@gmail.com" };

            // Act
            var response = await _userServices.ReadUser(request, It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(response, Is.EqualTo(responseExpected));
        }

        [Test]
        public async Task ReadAllUser_ShouldReturnAllUserData()
        {
            // Act
            GetAllResponse response = await _userServices.ListUser(new GetAllRequest(), It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task CreateUserandDeleteUser_ShouldCreateUsertoDatabaseThenDeleteUser()
        {
            // Arrange
            int id = (int)GetLatestIdAsync("Users");
            CreateUserRequest requestCreateUser = new() { Name = "Kera Sakti", Email = "monkeyking@gmail.com" };

            ReadUserRequest requestReadUser = new() { Id = id, DataThatNeeded = new FieldMask { Paths = { "id", "name", "email" } } };
            ReadUserResponse responseExpected = new() { Id = id, Name = "Kera Sakti", Email = "monkeyking@gmail.com" };

            // Act 1 Create User Then Read It
            await _userServices.CreateUser(requestCreateUser, It.IsAny<ServerCallContext>());
            ReadUserResponse responseReadUser = await _userServices.ReadUser(requestReadUser, It.IsAny<ServerCallContext>());
            // Assert Act 1, User Should be in The Database
            Assert.That(responseReadUser, Is.EqualTo(responseExpected));

            // Act 2 Delete User
            DeleteUserResponse responseDelete = await _userServices.DeleteUser(new DeleteUserRequest { Id = id }, It.IsAny<ServerCallContext>());
            // Assert Act 2
            Assert.That(responseDelete, Is.EqualTo(new DeleteUserResponse { Id = id }));

        }

        [Test]
        public async Task UpdateUser_ShouldUpdateUserData()
        {
            // Arrange Update User
            UpdateUserRequest requestUpdateUser = new() { Id = 6, Name = "amet officia Excepteur", Email = "ipsum@gmail.com" };
            // Arrange Read User
            ReadUserRequest requestReadUser = new() { Id = 6, DataThatNeeded = new FieldMask { Paths = { "id", "name", "email" } } };
            ReadUserResponse responseExpected = new() { Id = 6, Name = "amet officia Excepteur", Email = "ipsum@gmail.com" };

            // Act Update User
            await _userServices.UpdateUser(requestUpdateUser, It.IsAny<ServerCallContext>());
            // Act Read User
            ReadUserResponse responseReadUser = await _userServices.ReadUser(requestReadUser, It.IsAny<ServerCallContext>());

            // Assert
            Assert.That(responseReadUser, Is.EqualTo(responseExpected));

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
