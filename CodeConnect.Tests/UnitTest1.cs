using CodeConnect.Areas.Identity.Data;
using CodeConnect.Controllers;
using CodeConnect.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CodeConnect.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

        }

        [Fact]
        public async Task JoinProject_ProjectNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var store = new Mock<IUserStore<ApplicationUser>>(); // Correctly declared before use
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            var mockRepo = new Mock<IChatRepository>();
            var controller = new HomeController(null, null, mockUserManager.Object, mockRepo.Object);

            mockRepo.Setup(x => x.JoinProjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ThrowsAsync(new Exception("Project not found."));

            // Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => controller.JoinProject("testProject", "testPassword"));
            Assert.Equal("Project not found.", result.Message);
        }

    }
}