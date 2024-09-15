using Microsoft.EntityFrameworkCore;
using UploadThings.Data;
using UploadThings.Models;
using UploadThings.Repositories.SettingRepositories;

namespace UploadThingsTestProject
{
    [TestFixture]
    public class TransactionRepositoryTest
    {
        private PostgresContext _context;
        private TransactionRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PostgresContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryUsersDb")
                .Options;

            _context = new PostgresContext(options);
            _repository = new TransactionRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.transactions.Add(new Transaction { username = "Test Transaction", productname = "Bag" });
            _context.transactions.Add(new Transaction { username = "Another Transaction", productname = "Sarung" });
            _context.SaveChanges();
        }

        [TearDown]
        public void Teardown() { _context.Dispose(); }

        [Test]
        public async Task AddAsync_Should_Add_User()
        {
            var transaction = new Transaction { username = "John Doe", productname = "bag" };

            await _repository.AddAsync(transaction);

            var addedUser = await _context.transactions.FirstOrDefaultAsync(u => u.username == "John Doe");
            Assert.IsNotNull(addedUser);
            Assert.AreEqual("John Doe", addedUser.username);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Transaction_When_Exists()
        {
            // Arrange
            var transactionId = 1;

            // Act
            var transaction = await _repository.GetByIdAsync(transactionId);

            // Assert
            Assert.IsNotNull(transaction);
            Assert.AreEqual(transactionId, transaction.id);
            Assert.AreEqual("Test Transaction", transaction.username);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
        {
            // Arrange
            var transactionId = 999;

            // Act
            var transaction = await _repository.GetByIdAsync(transactionId);

            // Assert
            Assert.IsNull(transaction);
        }

        // Additional tests for GetByIdAsync, UpdateAsync, and DeleteAsync...
    }
}
