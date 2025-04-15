using System;
using AutoMapper;
using ClanService.DTOs;
using ClanService.Interfaces;
using ClanService.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ClanService.Controllers.Tests
{
    [TestClass]
    public class ChannelControllerTest
    {
        private Mock<IChannelService> _mockChannelService;
        private Mock<IMapper> _mockMapper;
        private ChannelController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Create mocks for dependencies
            _mockChannelService = new Mock<IChannelService>();
            _mockMapper = new Mock<IMapper>();

            // Create the controller with mocked dependencies
            _controller = new ChannelController(_mockChannelService.Object, _mockMapper.Object);
        }

        [TestMethod]
        public async Task CreateChannel_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var channelCreateDto = new ChannelCreateDto
            {
                Name = "Test Channel",
                ClanId = Guid.NewGuid()
            };

            var channel = new Channel
            {
                ChannelId = Guid.NewGuid(),
                Name = "Test Channel",
                ClanId = channelCreateDto.ClanId
            };

            var channelReadDto = new ChannelReadDto
            {
                ChannelId = channel.ChannelId,
                Name = channel.Name,
                ClanId = channel.ClanId
            };

            // Setup mocks
            _mockMapper.Setup(m => m.Map<Channel>(It.IsAny<ChannelCreateDto>())).Returns(channel);
            _mockChannelService.Setup(s => s.CreateChannelAsync(It.IsAny<Channel>()))
                .ReturnsAsync((channel, string.Empty));
            _mockMapper.Setup(m => m.Map<ChannelReadDto>(It.IsAny<Channel>())).Returns(channelReadDto);

            // Act
            var result = await _controller.CreateChannel(channelCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(channelReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task CreateChannel_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var channelCreateDto = new ChannelCreateDto(); // Invalid data
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreateChannel(channelCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateChannel_ServiceReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var channelCreateDto = new ChannelCreateDto
            {
                Name = "Test Channel",
                ClanId = Guid.NewGuid()
            };

            var channel = new Channel
            {
                Name = "Test Channel",
                ClanId = channelCreateDto.ClanId
            };

            // Setup mocks
            _mockMapper.Setup(m => m.Map<Channel>(It.IsAny<ChannelCreateDto>())).Returns(channel);
            _mockChannelService.Setup(s => s.CreateChannelAsync(It.IsAny<Channel>()))
                .ReturnsAsync((null, "Clan not found"));

            // Act
            var result = await _controller.CreateChannel(channelCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task GetChannelById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            var channel = new Channel
            {
                ChannelId = channelId,
                Name = "Test Channel",
                ClanId = Guid.NewGuid()
            };
            
            var channelReadDto = new ChannelReadDto
            {
                ChannelId = channel.ChannelId,
                Name = channel.Name,
                ClanId = channel.ClanId
            };

            _mockChannelService.Setup(s => s.GetChannelByIdAsync(channelId)).ReturnsAsync(channel);
            _mockMapper.Setup(m => m.Map<ChannelReadDto>(It.IsAny<Channel>())).Returns(channelReadDto);

            // Act
            var result = await _controller.GetChannelById(channelId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(channelReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetChannelById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            _mockChannelService.Setup(s => s.GetChannelByIdAsync(channelId)).ReturnsAsync((Channel)null);

            // Act
            var result = await _controller.GetChannelById(channelId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task GetChannelsByClanId_ExistingClanId_ReturnsOkResult()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var channels = new List<Channel>
            {
                new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 1", ClanId = clanId },
                new Channel { ChannelId = Guid.NewGuid(), Name = "Channel 2", ClanId = clanId }
            };

            var channelDtos = new List<ChannelReadDto>
            {
                new ChannelReadDto { ChannelId = channels[0].ChannelId, Name = channels[0].Name, ClanId = channels[0].ClanId },
                new ChannelReadDto { ChannelId = channels[1].ChannelId, Name = channels[1].Name, ClanId = channels[1].ClanId }
            };

            _mockChannelService.Setup(s => s.GetChannelsByClanIdAsync(clanId)).ReturnsAsync(channels);
            _mockMapper.Setup(m => m.Map<List<ChannelReadDto>>(It.IsAny<List<Channel>>())).Returns(channelDtos);

            // Act
            var result = await _controller.GetChannelsByClanId(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(channelDtos, okResult.Value);
        }

        [TestMethod]
        public async Task GetChannelsByClanId_NonExistingClanId_ReturnsNotFound()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            _mockChannelService.Setup(s => s.GetChannelsByClanIdAsync(clanId)).ReturnsAsync((List<Channel>)null);

            // Act
            var result = await _controller.GetChannelsByClanId(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateChannel_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var channelUpdateDto = new ChannelUpdateDto
            {
                ChannelId = Guid.NewGuid(),
                Name = "Updated Channel"
            };

            var existingChannel = new Channel
            {
                ChannelId = channelUpdateDto.ChannelId,
                Name = "Original Channel",
                ClanId = Guid.NewGuid()
            };

            var updatedChannel = new Channel
            {
                ChannelId = existingChannel.ChannelId,
                Name = channelUpdateDto.Name,
                ClanId = existingChannel.ClanId
            };

            var channelReadDto = new ChannelReadDto
            {
                ChannelId = updatedChannel.ChannelId,
                Name = updatedChannel.Name,
                ClanId = updatedChannel.ClanId
            };

            _mockChannelService.Setup(s => s.GetChannelByIdAsync(channelUpdateDto.ChannelId)).ReturnsAsync(existingChannel);
            _mockChannelService.Setup(s => s.UpdateChannelAsync(It.IsAny<Channel>())).ReturnsAsync(updatedChannel);
            _mockMapper.Setup(m => m.Map<ChannelReadDto>(It.IsAny<Channel>())).Returns(channelReadDto);

            // Act
            var result = await _controller.UpdateChannel(channelUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(channelReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateChannel_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var channelUpdateDto = new ChannelUpdateDto(); // Invalid data
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.UpdateChannel(channelUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateChannel_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var channelUpdateDto = new ChannelUpdateDto
            {
                ChannelId = Guid.NewGuid(),
                Name = "Updated Channel"
            };

            _mockChannelService.Setup(s => s.GetChannelByIdAsync(channelUpdateDto.ChannelId)).ReturnsAsync((Channel)null);

            // Act
            var result = await _controller.UpdateChannel(channelUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteChannel_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            _mockChannelService.Setup(s => s.DeleteChannelAsync(channelId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteChannel(channelId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteChannel_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var channelId = Guid.NewGuid();
            _mockChannelService.Setup(s => s.DeleteChannelAsync(channelId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteChannel(channelId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
    }
}