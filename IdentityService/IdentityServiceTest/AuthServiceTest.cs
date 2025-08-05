using System.Net;
using IdentityService.Data;
using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Messaging;
using IdentityService.Models;
using IdentityService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IdentityServiceTest;

[TestClass]
public class AuthServiceTest
{
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<IConfiguration> _configMock;
    private Mock<IdentityProducer> _messagePublisherMock;
    private Mock<ILogger<AuthService>> _loggerMock; // Fixed missing closing bracket
    private Mock<IIpAddressService> _ipAddressServiceMock;
    private Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private Mock<IdentityDbContext> _contextMock;
    private AuthService _authService;

    [TestInitialize]
    public void Setup()
    {
        _userManagerMock = MockUserManager();
        _configMock = new Mock<IConfiguration>();
        _messagePublisherMock = new Mock<IdentityProducer>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _ipAddressServiceMock = new Mock<IIpAddressService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _contextMock = new Mock<IdentityDbContext>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _configMock.Object,
            _messagePublisherMock.Object,
            _loggerMock.Object,
            _contextMock.Object,
            _ipAddressServiceMock.Object,
            _refreshTokenServiceMock.Object
        );
    }

    private Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
 return new Mock<UserManager<ApplicationUser>>(
        store.Object, null, null, null, null, null, null, null, null);    }

    [TestMethod]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessResponse()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "test-id",
            UserName = "testuser",
            Email = "testuser@example.com",
        };
        var loginModel = new LoginRequestModel
        {
            UserName = user.UserName,
            Password = "Test@123",
            DeviceInfo = "TestDevice",
        };
        var refreshTokenResult = ApiResponse<RefreshTokenResultDto>.Success(
            new RefreshTokenResultDto
            {
                RefreshToken = "refresh-token", // Fixed: RefreshTokenResultDto only has RefreshToken property
            }
        );

        _userManagerMock.Setup(x => x.FindByNameAsync(loginModel.UserName)).ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, loginModel.Password))
            .ReturnsAsync(true);
        _userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);

        _refreshTokenServiceMock
            .Setup(x =>
                x.CreateUserRefreshTokenAsync(user.Id, loginModel.DeviceInfo, It.IsAny<string>())
            )
            .ReturnsAsync(refreshTokenResult);

        _ipAddressServiceMock.Setup(x => x.GetClientIpAddress()).Returns("127.0.0.1");

        // Act
        var result = await _authService.LoginAsync(loginModel);

        // Assert
        Assert.IsTrue(result.IsSuccessfull);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual(user.Id, result.Data.UserID);
        Assert.IsNotNull(result.Data.AccessToken);
        Assert.AreEqual("refresh-token", result.Data.RefreshToken);
    }

    [TestMethod]
    public async Task LoginAsync_UserNotFound_ReturnsFailedResponse()
    {
        // Arrange
        var loginModel = new LoginRequestModel { UserName = "nonexistent", Password = "password" };
        _userManagerMock
            .Setup(x => x.FindByNameAsync(loginModel.UserName))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _authService.LoginAsync(loginModel);

        // Assert
        Assert.IsFalse(result.IsSuccessfull);
        Assert.AreEqual("There is no user with this username", result.Message);
    }

    [TestMethod]
    public async Task LoginAsync_InvalidPassword_ReturnsFailedResponse()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "testuser" };
        var loginModel = new LoginRequestModel
        {
            UserName = "testuser",
            Password = "wrongpassword",
        };

        _userManagerMock.Setup(x => x.FindByNameAsync(loginModel.UserName)).ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, loginModel.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.LoginAsync(loginModel);

        // Assert
        Assert.IsFalse(result.IsSuccessfull);
        Assert.AreEqual("Invalid username or password", result.Message);
    }

    [TestMethod]
    public async Task LoginAsync_AccountLocked_ReturnsUnauthorizedResponse()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "testuser" };
        var loginModel = new LoginRequestModel { UserName = "testuser", Password = "password" };

        _userManagerMock.Setup(x => x.FindByNameAsync(loginModel.UserName)).ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, loginModel.Password))
            .ReturnsAsync(true);
        _userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);

        // Act
        var result = await _authService.LoginAsync(loginModel);

        // Assert
        Assert.IsFalse(result.IsSuccessfull);
        Assert.AreEqual("Account locked", result.Message);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
    {
        // Arrange
        var user = new ApplicationUser { Id = "test-id" };
        var refreshTokenModel = new RefreshTokenDto
        {
            RefreshToken = "valid-token",
            DeviceInfo = "device",
        };
        var validRefreshToken = new UserRefreshToken { User = user, UserId = "test-id" };
        var newRefreshTokenResult = ApiResponse<RefreshTokenResultDto>.Success(
            new RefreshTokenResultDto { RefreshToken = "new-refresh-token" },
            "Success"
        );

        _refreshTokenServiceMock
            .Setup(x => x.GetValidRefreshTokenAsync(refreshTokenModel.RefreshToken))
            .ReturnsAsync(validRefreshToken);
        _refreshTokenServiceMock
            .Setup(x => x.RevokeRefreshTokenAsync(refreshTokenModel.RefreshToken))
            .Returns((Task<bool>)Task.CompletedTask);
        _refreshTokenServiceMock
            .Setup(x =>
                x.CreateUserRefreshTokenAsync(
                    validRefreshToken.UserId,
                    refreshTokenModel.DeviceInfo,
                    It.IsAny<string>()
                )
            )
            .ReturnsAsync(newRefreshTokenResult);
        _ipAddressServiceMock.Setup(x => x.GetClientIpAddress()).Returns("127.0.0.1");

        // Act
        var result = await _authService.RefreshTokenAsync(refreshTokenModel);

        // Assert
        Assert.IsTrue(result.IsSuccessfull);
        Assert.AreEqual("new-refresh-token", result.Data.RefreshToken);
    }

    [TestMethod]
    public async Task RefreshTokenAsync_InvalidToken_ReturnsUnauthorizedResponse()
    {
        // Arrange
        var refreshTokenModel = new RefreshTokenDto { RefreshToken = "invalid-token" };
        _refreshTokenServiceMock
            .Setup(x => x.GetValidRefreshTokenAsync(refreshTokenModel.RefreshToken))
            .ReturnsAsync((UserRefreshToken)null);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshTokenModel);

        // Assert
        Assert.IsFalse(result.IsSuccessfull);
        Assert.AreEqual("Invalid refresh token", result.Message);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task LogoutSessionAsync_ValidSession_ReturnsSuccessResponse()
    {
        // Arrange
        var sessionId = "session-id";
        var session = new UserRefreshToken { Id = sessionId };
        var mockSet = new Mock<DbSet<UserRefreshToken>>();

        _contextMock.Setup(x => x.UserRefreshTokens).Returns(mockSet.Object);
        _contextMock.Setup(x => x.UserRefreshTokens.FindAsync(sessionId)).ReturnsAsync(session);
        _contextMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _authService.LogoutSessionAsync(sessionId);

        // Assert
        Assert.IsTrue(result.IsSuccessfull);
        Assert.AreEqual("Session logged out successfully", result.Message);
        mockSet.Verify(x => x.Remove(session), Times.Once);
    }

    [TestMethod]
    public async Task LogoutSessionAsync_SessionNotFound_ReturnsNotFoundResponse()
    {
        // Arrange
        var sessionId = "999"; // Fixed: Changed to string to match method signature
        _contextMock
            .Setup(x => x.UserRefreshTokens.FindAsync(sessionId))
            .ReturnsAsync((UserRefreshToken)null);

        // Act
        var result = await _authService.LogoutSessionAsync(sessionId);

        // Assert
        Assert.IsFalse(result.IsSuccessfull);
        Assert.AreEqual("Session not found", result.Message);
        Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task LogoutSessionAsync_InactiveSession_ReturnsBadRequestResponse()
    {
        // Arrange
        var sessionId = "1"; // Fixed: Changed to string to match method signature
        var session = new UserRefreshToken { Id = sessionId };
        _contextMock.Setup(x => x.UserRefreshTokens.FindAsync(sessionId)).ReturnsAsync(session);

        // Act
        var result = await _authService.LogoutSessionAsync(sessionId);

        // Assert
        Assert.IsFalse(result.IsSuccessfull);
        Assert.AreEqual("Session is already inactive", result.Message);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public async Task GetMySessionsByUserId_WithSessions_ReturnsSessionsList()
    {
        // Arrange
        var userId = "test-user-id";
        var sessions = new List<UserRefreshToken>
        {
            new UserRefreshToken
            {
                Id = "1",
                UserId = userId,
                DeviceInfo = "Device1",
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = "127.0.0.1",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1),
            },
        }.AsQueryable();

        var mockSet = new Mock<DbSet<UserRefreshToken>>();
        mockSet
            .As<IQueryable<UserRefreshToken>>()
            .Setup(m => m.Provider)
            .Returns(sessions.Provider);
        mockSet
            .As<IQueryable<UserRefreshToken>>()
            .Setup(m => m.Expression)
            .Returns(sessions.Expression);
        mockSet
            .As<IQueryable<UserRefreshToken>>()
            .Setup(m => m.ElementType)
            .Returns(sessions.ElementType);
        mockSet
            .As<IQueryable<UserRefreshToken>>()
            .Setup(m => m.GetEnumerator())
            .Returns(sessions.GetEnumerator());

        _contextMock.Setup(x => x.UserRefreshTokens).Returns(mockSet.Object);

        // Act
        var result = await _authService.GetMySessionsByUserId(userId);

        // Assert
        Assert.IsTrue(result.IsSuccessfull);
        Assert.HasCount(1, result.Data);
        Assert.AreEqual("1", result.Data.First().Id);
    }

    [TestMethod]
    public async Task GetMySessionsByUserId_NoSessions_ReturnsNotFoundResponse()
    {
        // Arrange
        var userId = "test-user-id";
        var sessions = new List<UserRefreshToken>().AsQueryable();

        var mockSet = new Mock<DbSet<UserRefreshToken>>();
        mockSet
            .As<IQueryable<UserRefreshToken>>()
            .Setup(m => m.Provider)
            .Returns(sessions.Provider);
        mockSet
            .As<IQueryable<UserRefreshToken>>()
            .Setup(m => m.Expression)
            .Returns(sessions.Expression);
        mockSet
            .As<IQueryable<UserRefreshToken>>()
            .Setup(m => m.ElementType)
            .Returns(sessions.ElementType);
        mockSet
            .As<IQueryable<UserRefreshToken>>()
            .Setup(m => m.GetEnumerator())
            .Returns(sessions.GetEnumerator());

        _contextMock.Setup(x => x.UserRefreshTokens).Returns(mockSet.Object);

        // Act
        var result = await _authService.GetMySessionsByUserId(userId);

        // Assert
        Assert.IsFalse(result.IsSuccessfull);
        Assert.AreEqual("No sessions found for this user", result.Message);
        Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
    }
}
