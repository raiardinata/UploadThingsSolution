using Microsoft.EntityFrameworkCore;
using UploadThings.Data;
using UploadThings.Models;
using UploadThings.Repositories.SettingRepositories;

namespace UploadThingsTestProject
{
    [TestFixture]
    public class ProductRepositoryTest
    {
        private MariaDBContext _context;
        private ProductRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MariaDBContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryProductsDb")
                .Options;

            _context = new MariaDBContext(options);
            _repository = new ProductRepository(_context);

            SeedDatabase();
        }
        private void SeedDatabase()
        {
            _context.Products.Add(new Product { ProductName = "Test Product", Price = 1, ProductImagePath = "pathTest", TypeOfProduct = "bag" });
            _context.Products.Add(new Product { ProductName = "Another Product", Price = 1, ProductImagePath = "pathTest", TypeOfProduct = "sarung" });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown() { _context.Dispose(); }

        [Test]
        public async Task AddAsync_Should_Add_Product()
        {
            var product = new Product { ProductName = "Test Product", ProductImagePath = "test.jpg", TypeOfProduct = "Type1" };

            await _repository.AddAsync(product);

            var addedProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == "Test Product");
            Assert.IsNotNull(addedProduct);
            Assert.AreEqual("Test Product", addedProduct.ProductName);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Product_When_Exists()
        {
            // Arrange
            var productId = 1;

            // Act
            var product = await _repository.GetByIdAsync(productId);

            // Assert
            Assert.IsNotNull(product);
            Assert.AreEqual(productId, product.Id);
            Assert.AreEqual("Test Product", product.ProductName);
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
        {
            // Arrange
            var productId = 999;

            // Act
            var product = await _repository.GetByIdAsync(productId);

            // Assert
            Assert.IsNull(product);
        }

        // Additional tests for GetByIdAsync, UpdateAsync, and DeleteAsync...
    }
}
