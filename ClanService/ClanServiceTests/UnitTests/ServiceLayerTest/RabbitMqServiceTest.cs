using ClanService.Interfaces;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using ClanService.Services;
using Identity.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClanService.Services.Tests;

[TestClass]
public class RabbitMqServiceTest
{

    private Mock<IUserRepository> _mockUserRepository;
    private Mock<ILogger<RabbitMqService>> _mockLogger;
    private RabbitMqService _rabbitMqService;

    [TestInitialize]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<RabbitMqService>>();
        _rabbitMqService = new RabbitMqService(
                    _mockUserRepository.Object,
                    _mockLogger.Object
                );
    }
    [TestMethod]
    public async Task ConsumeUserInformation_ShouldAddUserToRepository()
    {
        // Arrange
        var userUpdatedMessage = new UserUpdatedMessage
        {
            userId = "test-user-id",
            userName = "TestUser",
            AvatarUrl = "test/avatar.jpg"
        };

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(new User { Id = "test-user-id", Username = "TestUser", AvatarUrl = "test/avatar.jpg" });

        // Act
        await _rabbitMqService.ConsumeUserInformation(userUpdatedMessage);

        // Assert
        _mockUserRepository.Verify(
            r => r.AddAsync(
                It.Is<User>(u =>
                    u.Id == "test-user-id" &&
                    u.Username == "TestUser" &&
                    u.AvatarUrl == "test/avatar.jpg")),
            Times.Once);

        // Verify that success is logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("User information saved successfully")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

}