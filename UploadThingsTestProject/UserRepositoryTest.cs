using Microsoft.EntityFrameworkCore;
using UploadThings.Data;
using UploadThings.Models;
using UploadThings.Repositories.SettingRepositories;

namespace UploadThingsTestProject
{
    [TestFixture]
    public class UserRepositoryTest
    {
        private MSSQLContext _context;
        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MSSQLContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryUsersDb")
                .Options;

            _context = new MSSQLContext(options);
            _repository = new UserRepository(_context);

            SeedDatabase();
        }
        private void SeedDatabase()
        {
            _context.Users.Add(new User { Name = "Test Name", Email = "Test Email" });
            _context.Users.Add(new User { Name = "Another Name", Email = "Another Test Email" });
            _context.SaveChanges();
        }

        [TearDown]
        public void Teardown() { _context.Dispose(); }

        [Test]
        public async Task AddAsync_Should_Add_User()
        {
            var user = new User { Name = "John Doe", Email = "john@example.com" };

            await _repository.AddAsync(user);

            var addedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "john@example.com");
            Assert.IsNotNull(addedUser);
            Assert.AreEqual("John Doe", addedUser.Name);
        }



        [Test]
        public async Task GetByIdAsync_Should_Return_User_When_Exists()
        {
            // Arrange
            var userId = 1;

            // Act
            var user = await _repository.GetByIdAsync(userId);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user.Id);
            Assert.AreEqual("Test Name", user.Name);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
        {
            // Arrange
            var userId = 999;

            // Act
            var user = await _repository.GetByIdAsync(userId);

            // Assert
            Assert.IsNull(user);
        }

        // Additional tests for GetByIdAsync, UpdateAsync, and DeleteAsync...
    }
}
