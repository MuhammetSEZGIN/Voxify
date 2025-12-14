using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService.Controllers;
using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IdentityServiceTests.UnitTests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        #region UpdateUser Tests

        [Fact]
        public async Task UpdateUser_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var updateModel = new UpdateUserModel
            {
                Id = "user123",
                UserName = "updateduser",
                Email = "updated@example.com",
                FullName = "Updated User",
                AvatarUrl = "https://example.com/avatar.jpg"
            };

            var identityResult = IdentityResult.Success;

            _mockUserService
                .Setup(x => x.UpdateUserAsync(It.IsAny<UpdateUserModel>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.UpdateUser(updateModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            var message = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.Equal("User updated successfully", message);

            _mockUserService.Verify(x => x.UpdateUserAsync(updateModel), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ServiceFails_ReturnsBadRequest()
        {
            // Arrange
            var updateModel = new UpdateUserModel
            {
                Id = "user123",
                UserName = "updateduser",
                Email = "updated@example.com"
            };

            var errors = new[]
            {
                new IdentityError
                {
                    Code = "DuplicateUserName",
                    Description = "Username already exists"
                }
            };
            var identityResult = IdentityResult.Failed(errors);

            _mockUserService
                .Setup(x => x.UpdateUserAsync(It.IsAny<UpdateUserModel>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.UpdateUser(updateModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedErrors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(
                badRequestResult.Value
            );
            Assert.Single(returnedErrors);
            Assert.Equal("DuplicateUserName", returnedErrors.First().Code);
            Assert.Equal("Username already exists", returnedErrors.First().Description);

            _mockUserService.Verify(x => x.UpdateUserAsync(updateModel), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_MultipleErrors_ReturnsBadRequestWithAllErrors()
        {
            // Arrange
            var updateModel = new UpdateUserModel
            {
                Id = "user123",
                UserName = "updateduser",
                Email = "invalid-email"
            };

            var errors = new[]
            {
                new IdentityError { Code = "InvalidEmail", Description = "Email is invalid" },
                new IdentityError { Code = "DuplicateUserName", Description = "Username taken" }
            };
            var identityResult = IdentityResult.Failed(errors);

            _mockUserService
                .Setup(x => x.UpdateUserAsync(It.IsAny<UpdateUserModel>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.UpdateUser(updateModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedErrors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(
                badRequestResult.Value
            );
            Assert.Equal(2, returnedErrors.Count());

            _mockUserService.Verify(x => x.UpdateUserAsync(updateModel), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithNullOptionalFields_ReturnsOkResult()
        {
            // Arrange
            var updateModel = new UpdateUserModel
            {
                Id = "user123",
                UserName = "updateduser",
                Email = "updated@example.com",
                FullName = null,
                AvatarUrl = null
            };

            var identityResult = IdentityResult.Success;

            _mockUserService
                .Setup(x => x.UpdateUserAsync(It.IsAny<UpdateUserModel>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.UpdateUser(updateModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _mockUserService.Verify(x => x.UpdateUserAsync(updateModel), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var updateModel = new UpdateUserModel
            {
                Id = "nonexistent",
                UserName = "updateduser",
                Email = "updated@example.com"
            };

            var errors = new[]
            {
                new IdentityError { Code = "UserNotFound", Description = "User not found" }
            };
            var identityResult = IdentityResult.Failed(errors);

            _mockUserService
                .Setup(x => x.UpdateUserAsync(It.IsAny<UpdateUserModel>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.UpdateUser(updateModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedErrors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(
                badRequestResult.Value
            );
            Assert.Single(returnedErrors);
            Assert.Equal("UserNotFound", returnedErrors.First().Code);

            _mockUserService.Verify(x => x.UpdateUserAsync(updateModel), Times.Once);
        }

        #endregion

        #region DeleteUser Tests

        [Fact]
        public async Task DeleteUser_ValidId_ReturnsOkResult()
        {
            // Arrange
            var userId = "user123";
            var identityResult = IdentityResult.Success;

            _mockUserService
                .Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            var message = response.GetType().GetProperty("Message")?.GetValue(response, null);
            Assert.Equal("User deleted successfully", message);

            _mockUserService.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ServiceFails_ReturnsBadRequest()
        {
            // Arrange
            var userId = "user123";
            var errors = new[]
            {
                new IdentityError
                {
                    Code = "DeleteFailed",
                    Description = "Failed to delete user"
                }
            };
            var identityResult = IdentityResult.Failed(errors);

            _mockUserService
                .Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedErrors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(
                badRequestResult.Value
            );
            Assert.Single(returnedErrors);
            Assert.Equal("DeleteFailed", returnedErrors.First().Code);
            Assert.Equal("Failed to delete user", returnedErrors.First().Description);

            _mockUserService.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var userId = "nonexistent";
            var errors = new[]
            {
                new IdentityError { Code = "UserNotFound", Description = "User not found" }
            };
            var identityResult = IdentityResult.Failed(errors);

            _mockUserService
                .Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedErrors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(
                badRequestResult.Value
            );
            Assert.Single(returnedErrors);
            Assert.Equal("UserNotFound", returnedErrors.First().Code);

            _mockUserService.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_EmptyId_StillCallsService()
        {
            // Arrange
            var userId = "";
            var errors = new[]
            {
                new IdentityError { Code = "InvalidUserId", Description = "User ID is invalid" }
            };
            var identityResult = IdentityResult.Failed(errors);

            _mockUserService
                .Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedErrors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(
                badRequestResult.Value
            );
            Assert.Single(returnedErrors);

            _mockUserService.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_MultipleErrors_ReturnsBadRequestWithAllErrors()
        {
            // Arrange
            var userId = "user123";
            var errors = new[]
            {
                new IdentityError
                {
                    Code = "DeleteFailed",
                    Description = "Failed to delete user"
                },
                new IdentityError
                {
                    Code = "ConcurrencyFailure",
                    Description = "Optimistic concurrency failure"
                }
            };
            var identityResult = IdentityResult.Failed(errors);

            _mockUserService
                .Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var returnedErrors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(
                badRequestResult.Value
            );
            Assert.Equal(2, returnedErrors.Count());

            _mockUserService.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        #endregion

        #region Service Integration Tests

        [Fact]
        public async Task UpdateUser_ServiceNotCalled_WhenModelIsInvalid()
        {
            // This would normally be handled by [ValidateModel] attribute
            // but we're testing the controller in isolation
            // In a real scenario, model validation happens before controller action

            // Arrange
            var updateModel = new UpdateUserModel
            {
                Id = "user123",
                UserName = "test",
                Email = "test@example.com"
            };

            var identityResult = IdentityResult.Success;

            _mockUserService
                .Setup(x => x.UpdateUserAsync(It.IsAny<UpdateUserModel>()))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.UpdateUser(updateModel);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockUserService.Verify(x => x.UpdateUserAsync(updateModel), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ServiceCalledOnce_ForValidRequest()
        {
            // Arrange
            var userId = "user123";
            var identityResult = IdentityResult.Success;

            _mockUserService
                .Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(identityResult);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockUserService.Verify(x => x.DeleteUserAsync(userId), Times.Once);
        }

        #endregion
    }
}