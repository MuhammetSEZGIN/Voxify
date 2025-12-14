using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Messaging;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IdentityServiceTests.UnitTests.Services
{
    public class RegisterServiceTests
    {
        private static IConfiguration BuildConfig() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "JWT:Key", "test-signing-key-12345678901234567890" }
                })
                .Build();

        private static Mock<UserManager<ApplicationUser>> BuildUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private RegisterService CreateService(
            Mock<UserManager<ApplicationUser>> userManagerMock,
            Mock<IRefreshTokenService> refreshTokenServiceMock,
            Mock<IIpAddressService> ipMock,
            Mock<IIdentityProducer> producerMock = null)
        {
            producerMock ??= new Mock<IIdentityProducer>();
            var logger = new Mock<ILogger<RegisterService>>();

            return new RegisterService(
                userManagerMock.Object,
                logger.Object,
                ipMock.Object,
                producerMock.Object,
                refreshTokenServiceMock.Object,
                BuildConfig());
        }

        [Fact]
        public async Task RegisterAsync_ReturnsTokens_WhenSuccessful()
        {
            // Arrange
            var userManager = BuildUserManager();
            var refreshService = new Mock<IRefreshTokenService>();
            var ipMock = new Mock<IIpAddressService>();
            var producerMock = new Mock<IIdentityProducer>();

            ApplicationUser capturedUser = null;
            userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, p) => capturedUser = u)
                .ReturnsAsync(IdentityResult.Success);

            refreshService
                .Setup(x => x.CreateUserRefreshTokenAsync(
                    It.IsAny<string>(),
                    "DeviceA",
                    "127.0.0.1"))
                .ReturnsAsync(ApiResponse<RefreshTokenResultDto>.Success(
                    new RefreshTokenResultDto
                    {
                        AccessToken = "new-access",
                        RefreshToken = "new-refresh"
                    }));

            ipMock.Setup(x => x.GetClientIpAddress()).Returns("127.0.0.1");

            producerMock
                .Setup(x => x.PublishUserUpdatedMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(userManager, refreshService, ipMock, producerMock);

            // Act
            var result = await service.RegisterAsync(new RegisterModel
            {
                UserName = "john",
                Email = "john@example.com",
                Password = "Pass@123",
                PasswordConfirmation = "Pass@123",
                DeviceInfo = "DeviceA",
                AvatarUrl = "https://example.com/avatar.jpg"
            });

            // Assert
            Assert.True(result.IsSuccessfull);
            Assert.NotNull(result.Data);
            Assert.Equal("new-access", result.Data.Token);
            Assert.Equal("new-refresh", result.Data.RefreshToken);
            Assert.NotNull(result.Data.UserId);
            Assert.Equal((int)System.Net.HttpStatusCode.Created, result.StatusCode);

            userManager.Verify(
                x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Pass@123"),
                Times.Once);
            producerMock.Verify(
                x => x.PublishUserUpdatedMessageAsync("john", "https://example.com/avatar.jpg", It.IsAny<string>()),
                Times.Once);
            refreshService.Verify(
                x => x.CreateUserRefreshTokenAsync(
                    It.IsAny<string>(),
                    "DeviceA",
                    "127.0.0.1"),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_Fails_WhenUserCreationFails()
        {
            // Arrange
            var userManager = BuildUserManager();
            var refreshService = new Mock<IRefreshTokenService>();
            var ipMock = new Mock<IIpAddressService>();

            var identityError = new IdentityError
            {
                Code = "DuplicateUserName",
                Description = "Username already exists"
            };

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            var service = CreateService(userManager, refreshService, ipMock);

            // Act
            var result = await service.RegisterAsync(new RegisterModel
            {
                UserName = "john",
                Email = "john@example.com",
                Password = "Pass@123",
                PasswordConfirmation = "Pass@123",
                DeviceInfo = "DeviceA"
            });

            // Assert
            Assert.False(result.IsSuccessfull);
            Assert.Contains("Username already exists", result.Message);

            userManager.Verify(
                x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),
                Times.Once);
            refreshService.Verify(
                x => x.CreateUserRefreshTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_Fails_WithMultipleErrors()
        {
            // Arrange
            var userManager = BuildUserManager();
            var refreshService = new Mock<IRefreshTokenService>();
            var ipMock = new Mock<IIpAddressService>();

            var errors = new[]
            {
                new IdentityError { Code = "InvalidEmail", Description = "Email format is invalid" },
                new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" }
            };

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(errors));

            var service = CreateService(userManager, refreshService, ipMock);

            // Act
            var result = await service.RegisterAsync(new RegisterModel
            {
                UserName = "john",
                Email = "invalid-email",
                Password = "short",
                PasswordConfirmation = "short",
                DeviceInfo = "DeviceA"
            });

            // Assert
            Assert.False(result.IsSuccessfull);
            Assert.Contains("Email format is invalid", result.Message);
            Assert.Contains("Password is too short", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_Fails_WhenPublishMessageThrows()
        {
            // Arrange
            var userManager = BuildUserManager();
            var refreshService = new Mock<IRefreshTokenService>();
            var ipMock = new Mock<IIpAddressService>();
            var producerMock = new Mock<IIdentityProducer>();

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            producerMock
                .Setup(x => x.PublishUserUpdatedMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception("Message bus error"));

            var service = CreateService(userManager, refreshService, ipMock, producerMock);

            // Act
            var result = await service.RegisterAsync(new RegisterModel
            {
                UserName = "john",
                Email = "john@example.com",
                Password = "Pass@123",
                PasswordConfirmation = "Pass@123",
                DeviceInfo = "DeviceA"
            });

            // Assert
            Assert.False(result.IsSuccessfull);
            Assert.Equal((int)System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Contains("An error occurred while registering user", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_Fails_WhenRefreshTokenServiceFails()
        {
            // Arrange
            var userManager = BuildUserManager();
            var refreshService = new Mock<IRefreshTokenService>();
            var ipMock = new Mock<IIpAddressService>();
            var producerMock = new Mock<IIdentityProducer>();

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            refreshService
                .Setup(x => x.CreateUserRefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(ApiResponse<RefreshTokenResultDto>.Failed(
                    "Token service error",
                    null,
                    (int)System.Net.HttpStatusCode.InternalServerError));

            ipMock.Setup(x => x.GetClientIpAddress()).Returns("127.0.0.1");

            producerMock
                .Setup(x => x.PublishUserUpdatedMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(userManager, refreshService, ipMock, producerMock);

            // Act
            var result = await service.RegisterAsync(new RegisterModel
            {
                UserName = "john",
                Email = "john@example.com",
                Password = "Pass@123",
                PasswordConfirmation = "Pass@123",
                DeviceInfo = "DeviceA"
            });

            // Assert
            Assert.False(result.IsSuccessfull);
            Assert.Equal((int)System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public async Task RegisterAsync_Fails_OnGeneralException()
        {
            // Arrange
            var userManager = BuildUserManager();
            var refreshService = new Mock<IRefreshTokenService>();
            var ipMock = new Mock<IIpAddressService>();

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            var service = CreateService(userManager, refreshService, ipMock);

            // Act
            var result = await service.RegisterAsync(new RegisterModel
            {
                UserName = "john",
                Email = "john@example.com",
                Password = "Pass@123",
                PasswordConfirmation = "Pass@123",
                DeviceInfo = "DeviceA"
            });

            // Assert
            Assert.False(result.IsSuccessfull);
            Assert.Equal((int)System.Net.HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.Contains("An error occurred while registering user", result.Message);
            Assert.Contains("Database error", result.Errors.First());
        }

        [Fact]
        public async Task IsUsernameTakenAsync_ReturnsTrue_WhenUsernameExists()
        {
            // Arrange
            var userManager = BuildUserManager();
            var user = new ApplicationUser { Id = "u1", UserName = "john" };

            userManager
                .Setup(x => x.FindByNameAsync("john"))
                .ReturnsAsync(user);

            var service = CreateService(userManager, new Mock<IRefreshTokenService>(), new Mock<IIpAddressService>());

            // Act
            var result = await service.IsUsernameTakenAsync("john");

            // Assert
            Assert.True(result);
            userManager.Verify(x => x.FindByNameAsync("john"), Times.Once);
        }

        [Fact]
        public async Task IsUsernameTakenAsync_ReturnsFalse_WhenUsernameNotExists()
        {
            // Arrange
            var userManager = BuildUserManager();
            userManager
                .Setup(x => x.FindByNameAsync("ghost"))
                .ReturnsAsync((ApplicationUser)null);

            var service = CreateService(userManager, new Mock<IRefreshTokenService>(), new Mock<IIpAddressService>());

            // Act
            var result = await service.IsUsernameTakenAsync("ghost");

            // Assert
            Assert.False(result);
            userManager.Verify(x => x.FindByNameAsync("ghost"), Times.Once);
        }

        [Fact]
        public async Task IsEmailTakenAsync_ReturnsTrue_WhenEmailExists()
        {
            // Arrange
            var userManager = BuildUserManager();
            var user = new ApplicationUser { Id = "u1", Email = "john@example.com" };

            userManager
                .Setup(x => x.FindByEmailAsync("john@example.com"))
                .ReturnsAsync(user);

            var service = CreateService(userManager, new Mock<IRefreshTokenService>(), new Mock<IIpAddressService>());

            // Act
            var result = await service.IsEmailTakenAsync("john@example.com");

            // Assert
            Assert.True(result);
            userManager.Verify(x => x.FindByEmailAsync("john@example.com"), Times.Once);
        }

        [Fact]
        public async Task IsEmailTakenAsync_ReturnsFalse_WhenEmailNotExists()
        {
            // Arrange
            var userManager = BuildUserManager();
            userManager
                .Setup(x => x.FindByEmailAsync("ghost@example.com"))
                .ReturnsAsync((ApplicationUser)null);

            var service = CreateService(userManager, new Mock<IRefreshTokenService>(), new Mock<IIpAddressService>());

            // Act
            var result = await service.IsEmailTakenAsync("ghost@example.com");

            // Assert
            Assert.False(result);
            userManager.Verify(x => x.FindByEmailAsync("ghost@example.com"), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_SetsEmailConfirmedToFalse()
        {
            // Arrange
            var userManager = BuildUserManager();
            var refreshService = new Mock<IRefreshTokenService>();
            var ipMock = new Mock<IIpAddressService>();
            var producerMock = new Mock<IIdentityProducer>();

            ApplicationUser capturedUser = null;
            userManager
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Callback<ApplicationUser, string>((u, p) => capturedUser = u)
                .ReturnsAsync(IdentityResult.Success);

            refreshService
                .Setup(x => x.CreateUserRefreshTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(ApiResponse<RefreshTokenResultDto>.Success(
                    new RefreshTokenResultDto
                    {
                        AccessToken = "token",
                        RefreshToken = "refresh"
                    }));

            ipMock.Setup(x => x.GetClientIpAddress()).Returns("127.0.0.1");

            producerMock
                .Setup(x => x.PublishUserUpdatedMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(userManager, refreshService, ipMock, producerMock);

            // Act
            var result = await service.RegisterAsync(new RegisterModel
            {
                UserName = "john",
                Email = "john@example.com",
                Password = "Pass@123",
                PasswordConfirmation = "Pass@123",
                DeviceInfo = "DeviceA"
            });

            // Assert
            Assert.False(capturedUser.EmailConfirmed);
            Assert.Equal("john", capturedUser.UserName);
            Assert.Equal("john@example.com", capturedUser.Email);
        }
    }
}