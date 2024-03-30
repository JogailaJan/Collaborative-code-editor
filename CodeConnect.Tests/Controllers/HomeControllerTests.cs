using CodeConnect.Areas.Identity.Data;
using CodeConnect.Controllers;
using CodeConnect.Infrastructure.Repository;
using CodeConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConnect.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<IChatRepository> _mockRepo;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        public HomeControllerTests()
        {
            _mockRepo = new Mock<IChatRepository>();
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            // Mock other dependencies as needed.
            _controller = new HomeController(null, null, _mockUserManager.Object, _mockRepo.Object);
        }

        [Theory]
        [InlineData("Project1", "Password1", true)]
        [InlineData("Project1", "WrongPassword", false)]
        [InlineData("NonexistentProject", "Password1", false)]
        public async Task JoinProject_ReturnsExpectedResult(string projectName, string password, bool shouldSucceed)
        {
            // Arrange
            var userId = "testUser";
            if (shouldSucceed)
            {
                _mockRepo.Setup(r => r.JoinProjectAsync(projectName, password, userId)).ReturnsAsync(new Project());
            }
            else
            {
                _mockRepo.Setup(r => r.JoinProjectAsync(projectName, password, userId))
                         .ThrowsAsync(new Exception("Project not found or password incorrect"));
            }

            // Act & Assert
            if (shouldSucceed)
            {
                var result = await _controller.JoinProject(projectName, password);
                Assert.IsType<RedirectToActionResult>(result);
            }
            else
            {
                await Assert.ThrowsAsync<Exception>(() => _controller.JoinProject(projectName, password));
            }
        }

        [Fact]
        public async Task Index_LoadsProjects_WithinExpectedTime()
        {
            // Arrange
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            var result = await _controller.IndexAsync();
            stopwatch.Stop();

            // Assert
            Assert.IsType<ViewResult>(result);
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Loading projects took longer than 1000 ms");
        }

        [Fact]
        public async Task DeleteProject_ValidProjectId_DoesNotThrowException()
        {
            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _controller.DeleteProject(1));
            Assert.Null(exception);
        }


        [Fact]
        public async Task Index_ReturnsProjects_DoesNotContainDeletedProject()
        {
            // Arrange
            var deletedProjectName = "Obsolete Project";

            // Act
            var result = await _controller.IndexAsync() as ViewResult;

            // Assert
            var projects = Assert.IsAssignableFrom<IEnumerable<Project>>(result.Model);
            Assert.DoesNotContain(projects, p => p.Name == deletedProjectName);
        }

        [Fact]
        public async Task Index_ReturnsProjects_ContainsSpecificProject()
        {
            // Arrange
            var expectedProjectName = "Project X";

            // Act
            var result = await _controller.IndexAsync() as ViewResult;

            // Assert
            var projects = Assert.IsAssignableFrom<IEnumerable<Project>>(result.Model);
            Assert.Contains(projects, p => p.Name == expectedProjectName);
        }

        [Fact]
        [Trait("Category", "Performance")]
        public async Task ProjectLoadingPerformanceTest()
        {
            // Arrange
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            var result = await _controller.IndexAsync();
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Loading projects took too long.");
        }

        [Fact(Skip = "Skipping this test temporarily due to external API changes.")]
        public async Task ExternalApiIntegrationTest()
        {
            // Test logic that depends on an external API
        }

    }


}
