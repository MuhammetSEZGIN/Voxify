using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClanService.DTOs;
using ClanService.Interfaces.Repositories;
using ClanService.Interfaces.Services;
using ClanService.Models;
using ClanService.RabbitMq;
using ClanService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClanService.Services.Tests
{
    [TestClass]
    public class ChannelServiceTest
    {
        private Mock<IClanRepository> _mockClanRepository;
        private Mock<IChannelRepository> _mockChannelRepository;
        private Mock<ILogger<ChannelService>> _mockLogger;
        private Mock<IClanServicePublisher> _mockPublisher;
        private ChannelService _channelService;

        [TestInitialize]
        public void Setup()
        {
            _mockClanRepository = new Mock<IClanRepository>();
            _mockChannelRepository = new Mock<IChannelRepository>();
            _mockLogger = new Mock<ILogger<ChannelService>>();
            _mockPublisher = new Mock<IClanServicePublisher>();

            _channelService = new ChannelService(
                _mockClanRepository.Object,
                _mockChannelRepository.Object,
                _mockLogger.Object,
                _mockPublisher.Object
            );
        }

        [TestMethod]
        public async Task CreateChannelAsync_WithValidClan_ShouldCreateChannel()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var channel = new Channel
            {
                Name = "Test Channel",
                ClanId = clanId
            };

            var clan = new Clan
            {
                ClanId = clanId,
                Name = "Test Clan"
            };

            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            _mockChannelRepository.Setup(r=>r.AddAsync(It.IsAny<Channel>()))
                .ReturnsAsync((Channel c)=>{
                    c.ChannelId = Guid.NewGuid();
                    return c;
                });

            // Act
            var result = await _channelService.CreateChannelAsync(channel);

            // Assert
            Assert.IsNotNull(result.Item1, "Channel should not be null");
            Assert.IsTrue(result.Item1.ChannelId != Guid.Empty, "Channel should have a valid ID");
            Assert.AreEqual("Channel created successfully", result.Item2);
            
            _mockChannelRepository.Verify(r => r.AddAsync(It.IsAny<Channel>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateChannelAsync_WithInvalidClan_ShouldReturnNull()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var channel = new Channel
            {
                Name = "Test Channel",
                ClanId = clanId
            };

            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync((Clan)null);

            // Act
            var result = await _channelService.CreateChannelAsync(channel);

            // Assert
            Assert.IsNull(result.Item1, "Channel should be null");
            Assert.AreEqual("Clan not found", result.Item2);
            
            _mockChannelRepository.Verify(r => r.AddAsync(It.IsAny<Channel>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateChannelAsync_WhenExceptionThrown_ShouldReturnError()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var channel = new Channel
            {
                Name = "Test Channel",
                ClanId = clanId
            };

            var clan = new Clan
            {
                ClanId = clanId,
                Name = "Test Clan"
            };

            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            _mockChannelRepository.Setup(r => r.AddAsync(It.IsAny<Channel>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _channelService.CreateChannelAsync(channel);

            // Assert
            Assert.IsNull(result.Item1, "Channel should be null");
            Assert.AreEqual("Error while creating channel", result.Item2);
            
            // Verify that exception is logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetChannelByIdAsync_ShouldReturnChannel()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var channel = new Channel
            {
                ChannelId = channelId,
                Name = "Test Channel",
                ClanId = Guid.NewGuid()
            };

            _mockChannelRepository.Setup(r => r.GetByIdAsync(channelId)).ReturnsAsync(channel);

            // Act
            var result = await _channelService.GetChannelByIdAsync(channelId);

            // Assert
            Assert.IsNotNull(result, "Channel should not be null");
            Assert.AreEqual(channelId, result.ChannelId);
            Assert.AreEqual("Test Channel", result.Name);
        }

        [TestMethod]
        public async Task GetChannelByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var channelId = Guid.NewGuid();

            _mockChannelRepository.Setup(r => r.GetByIdAsync(channelId)).ReturnsAsync((Channel)null);

            // Act
            var result = await _channelService.GetChannelByIdAsync(channelId);

            // Assert
            Assert.IsNull(result, "Channel should be null");
        }

        [TestMethod]
        public async Task GetChannelsByClanIdAsync_ShouldReturnChannels()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var channels = new List<Channel>
            {
                new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 1", ClanId = clanId },
                new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 2", ClanId = clanId }
            };

            _mockChannelRepository.Setup(r => r.GetChannelsByClanIdAsync(clanId)).ReturnsAsync(channels);

            // Act
            var result = await _channelService.GetChannelsByClanIdAsync(clanId);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(2, result.Count, "Should return 2 channels");
            Assert.AreEqual("Channel 1", result[0].Name);
            Assert.AreEqual("Channel 2", result[1].Name);
        }

        [TestMethod]
        public async Task GetChannelsByClanIdAsync_WithNoChannels_ShouldReturnEmptyList()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var emptyList = new List<Channel>();

            _mockChannelRepository.Setup(r => r.GetChannelsByClanIdAsync(clanId)).ReturnsAsync(emptyList);

            // Act
            var result = await _channelService.GetChannelsByClanIdAsync(clanId);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(0, result.Count, "Should return empty list");
        }

        [TestMethod]
        public async Task UpdateChannelAsync_ShouldUpdateAndReturnChannel()
        {
            // Arrange
            var channel = new Channel
            {
                ChannelId = Guid.NewGuid(),
                Name = "Updated Channel",
                ClanId = Guid.NewGuid()
            };

            _mockChannelRepository.Setup(r => r.UpdateAsync(channel)).Returns(Task.CompletedTask);

            // Act
            var result = await _channelService.UpdateChannelAsync(channel);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(channel.ChannelId, result.ChannelId);
            Assert.AreEqual("Updated Channel", result.Name);
            
            _mockChannelRepository.Verify(r => r.UpdateAsync(channel), Times.Once);
        }

        [TestMethod]
        public async Task UpdateChannelAsync_WhenExceptionThrown_ShouldReturnNull()
        {
            // Arrange
            var channel = new Channel
            {
                ChannelId = Guid.NewGuid(),
                Name = "Updated Channel",
                ClanId = Guid.NewGuid()
            };

            _mockChannelRepository.Setup(r => r.UpdateAsync(channel))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _channelService.UpdateChannelAsync(channel);

            // Assert
            Assert.IsNull(result, "Result should be null");
            
            // Verify that exception is logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteChannelAsync_WithExistingChannel_ShouldReturnTrue()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var channel = new Channel
            {
                ChannelId = channelId,
                Name = "Test Channel",
                ClanId = Guid.NewGuid()
            };

            _mockChannelRepository.Setup(r => r.GetByIdAsync(channelId)).ReturnsAsync(channel);
            _mockChannelRepository.Setup(r => r.DeleteAsync(channel)).Returns(Task.CompletedTask);
            _mockPublisher.Setup(p => p.PublishDeleteChannelMessageAsync(It.IsAny<ChannelDeletedMessage>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _channelService.DeleteChannelAsync(channelId);

            // Assert
            Assert.IsTrue(result, "Result should be true");
            
            _mockChannelRepository.Verify(r => r.DeleteAsync(channel), Times.Once);
            _mockPublisher.Verify(p => p.PublishDeleteChannelMessageAsync(
                It.Is<ChannelDeletedMessage>(m => m.ChannelId == channelId)), 
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteChannelAsync_WithNonExistentChannel_ShouldReturnFalse()
        {
            // Arrange
            var channelId = Guid.NewGuid();

            _mockChannelRepository.Setup(r => r.GetByIdAsync(channelId)).ReturnsAsync((Channel)null);

            // Act
            var result = await _channelService.DeleteChannelAsync(channelId);

            // Assert
            Assert.IsFalse(result, "Result should be false");
            
            _mockChannelRepository.Verify(r => r.DeleteAsync(It.IsAny<Channel>()), Times.Never);
            _mockPublisher.Verify(p => p.PublishDeleteChannelMessageAsync(It.IsAny<ChannelDeletedMessage>()), 
                Times.Never);
        }

        [TestMethod]
        public async Task DeleteChannelAsync_WhenExceptionThrown_ShouldReturnFalse()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var channel = new Channel
            {
                ChannelId = channelId,
                Name = "Test Channel",
                ClanId = Guid.NewGuid()
            };

            _mockChannelRepository.Setup(r => r.GetByIdAsync(channelId)).ReturnsAsync(channel);
            _mockChannelRepository.Setup(r => r.DeleteAsync(channel))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _channelService.DeleteChannelAsync(channelId);

            // Assert
            Assert.IsFalse(result, "Result should be false");
            
            // Verify that exception is logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}