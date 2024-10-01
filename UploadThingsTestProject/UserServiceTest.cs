using Moq;
using UploadThingsGrpcService.Domain.Entities;
using UploadThingsGrpcService.Domain.Interfaces;

namespace UploadThingsTestProject
{
    class UserServiceTest
    {
        private readonly Mock<IGeneralRepository<User>> _mockUserService;

        public UserServiceTest()
        {
            _mockUserService = new Mock<IGeneralRepository<User>>();
        }

        [Test]
        public async Task CreateUser_ShouldReturnCreatedUser()
        {
            var user = new User { Name = "John Doe", Email = "john.doe@test.com" };

            _mockUserService.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync(new Exception(null));

            var result = await _mockUserService.Object.AddAsync(user);

            if (result != null)
            {
                Assert.Fail(result.Message);
            }
            Assert.That(result, Is.EqualTo(null));

        }
    }
}
