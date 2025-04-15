using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using ClanService.Services;
using Moq;

namespace ClanService.Services.Tests
{
    [TestClass]
    public class VoiceChannelServiceTest
    {
        private Mock<IClanRepository> _mockClanRepository;
        private Mock<IVoiceChannelRepository> _mockVoiceChannelRepository;
        private VoiceChannelService _voiceChannelService;

        [TestInitialize]
        public void Setup()
        {
            _mockClanRepository = new Mock<IClanRepository>();
            _mockVoiceChannelRepository = new Mock<IVoiceChannelRepository>();
            
            _voiceChannelService = new VoiceChannelService(
                _mockClanRepository.Object,
                _mockVoiceChannelRepository.Object
            );
        }

        [TestMethod]
        public async Task CreateVoiceChannelAsync_WithValidClan_ShouldCreateChannel()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var voiceChannel = new VoiceChannel
            {
                Name = "Test Voice Channel",
                ClanId = clanId
            };
            
            var clan = new Clan { ClanId = clanId };
            
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync(clan);
            _mockVoiceChannelRepository.Setup(r => r.AddAsync(voiceChannel)).ReturnsAsync(voiceChannel);

            // Act
            var result = await _voiceChannelService.CreateVoiceChannelAsync(voiceChannel);

            // Assert
            Assert.IsNotNull(result.Item1);
            Assert.AreEqual("VoiceChannel created successfully", result.Item2);
            _mockVoiceChannelRepository.Verify(r => r.AddAsync(voiceChannel), Times.Once);
        }

        [TestMethod]
        public async Task CreateVoiceChannelAsync_WithInvalidClan_ShouldReturnNull()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var voiceChannel = new VoiceChannel
            {
                Name = "Test Voice Channel",
                ClanId = clanId
            };
            
            _mockClanRepository.Setup(r => r.GetByIdAsync(clanId)).ReturnsAsync((Clan)null);

            // Act
            var result = await _voiceChannelService.CreateVoiceChannelAsync(voiceChannel);

            // Assert
            Assert.IsNull(result.Item1);
            Assert.AreEqual("Clan not found", result.Item2);
            _mockVoiceChannelRepository.Verify(r => r.AddAsync(It.IsAny<VoiceChannel>()), Times.Never);
        }

        [TestMethod]
        public async Task GetVoiceChannelByIdAsync_ShouldReturnChannel()
        {
            // Arrange
            var voiceChannelId = Guid.NewGuid();
            var voiceChannel = new VoiceChannel
            {
                VoiceChannelId = voiceChannelId,
                Name = "Test Voice Channel"
            };
            
            _mockVoiceChannelRepository.Setup(r => r.GetByIdAsync(voiceChannelId)).ReturnsAsync(voiceChannel);

            // Act
            var result = await _voiceChannelService.GetVoiceChannelByIdAsync(voiceChannelId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(voiceChannelId, result.VoiceChannelId);
        }

        [TestMethod]
        public async Task GetVoiceChannelsByClanIdAsync_ShouldReturnChannels()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var voiceChannels = new List<VoiceChannel>
            {
                new VoiceChannel { VoiceChannelId = Guid.NewGuid(), Name = "Channel 1", ClanId = clanId },
                new VoiceChannel { VoiceChannelId = Guid.NewGuid(), Name = "Channel 2", ClanId = clanId }
            };
            
            _mockVoiceChannelRepository.Setup(r => r.GetVoiceChannelsByClanIdAsync(clanId))
                .ReturnsAsync(voiceChannels);

            // Act
            var result = await _voiceChannelService.GetVoiceChannelsByClanIdAsync(clanId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Channel 1", result[0].Name);
            Assert.AreEqual("Channel 2", result[1].Name);
        }

        [TestMethod]
        public async Task UpdateVoiceChannelAsync_ShouldUpdateAndReturnChannel()
        {
            // Arrange
            var voiceChannel = new VoiceChannel
            {
                VoiceChannelId = Guid.NewGuid(),
                Name = "Updated Voice Channel"
            };
            
            _mockVoiceChannelRepository.Setup(r => r.UpdateAsync(voiceChannel)).Returns(Task.CompletedTask);

            // Act
            var result = await _voiceChannelService.UpdateVoiceChannelAsync(voiceChannel);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(voiceChannel.VoiceChannelId, result.VoiceChannelId);
            Assert.AreEqual("Updated Voice Channel", result.Name);
            _mockVoiceChannelRepository.Verify(r => r.UpdateAsync(voiceChannel), Times.Once);
        }

        [TestMethod]
        public async Task DeleteVoiceChannelAsync_WithExistingChannel_ShouldReturnTrue()
        {
            // Arrange
            var voiceChannelId = Guid.NewGuid();
            var voiceChannel = new VoiceChannel
            {
                VoiceChannelId = voiceChannelId,
                Name = "Test Voice Channel"
            };
            
            _mockVoiceChannelRepository.Setup(r => r.GetByIdAsync(voiceChannelId)).ReturnsAsync(voiceChannel);
            _mockVoiceChannelRepository.Setup(r => r.DeleteAsync(voiceChannel)).Returns(Task.CompletedTask);

            // Act
            var result = await _voiceChannelService.DeleteVoiceChannelAsync(voiceChannelId);

            // Assert
            Assert.IsTrue(result);
            _mockVoiceChannelRepository.Verify(r => r.DeleteAsync(voiceChannel), Times.Once);
        }

        [TestMethod]
        public async Task DeleteVoiceChannelAsync_WithNonExistingChannel_ShouldReturnFalse()
        {
            // Arrange
            var voiceChannelId = Guid.NewGuid();
            
            _mockVoiceChannelRepository.Setup(r => r.GetByIdAsync(voiceChannelId)).ReturnsAsync((VoiceChannel)null);

            // Act
            var result = await _voiceChannelService.DeleteVoiceChannelAsync(voiceChannelId);

            // Assert
            Assert.IsFalse(result);
            _mockVoiceChannelRepository.Verify(r => r.DeleteAsync(It.IsAny<VoiceChannel>()), Times.Never);
        }
    }
}