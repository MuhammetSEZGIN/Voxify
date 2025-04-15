using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ClanService.Controllers;
using ClanService.DTOs;
using ClanService.Interfaces;
using ClanService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClanService.Controllers.Tests
{
    [TestClass]
    public class VoiceChannelControllerTest
    {
        private Mock<IVoiceChannelService> _mockVoiceChannelService;
        private Mock<IMapper> _mockMapper;
        private VoiceChannelController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Create mocks for dependencies
            _mockVoiceChannelService = new Mock<IVoiceChannelService>();
            _mockMapper = new Mock<IMapper>();

            // Create the controller with mocked dependencies
            _controller = new VoiceChannelController(
                _mockVoiceChannelService.Object,
                _mockMapper.Object);
        }

        [TestMethod]
        public async Task CreateVoiceChannel_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var voiceChannelCreateDto = new VoiceChannelCreateDto
            {
                Name = "Test Voice Channel",
                ClanId = Guid.NewGuid()
            };

            var voiceChannel = new VoiceChannel
            {
                VoiceChannelId = Guid.NewGuid(),
                Name = "Test Voice Channel",
                ClanId = voiceChannelCreateDto.ClanId,
                IsActive = true,
                MaxParticipants = 5
            };

            var voiceChannelReadDto = new VoiceChannelReadDto
            {
                VoiceChannelId = voiceChannel.VoiceChannelId,
                Name = voiceChannel.Name,
                ClanId = voiceChannel.ClanId,
                IsActive = voiceChannel.IsActive
            };

            // Setup mocks
            _mockMapper.Setup(m => m.Map<VoiceChannel>(It.IsAny<VoiceChannelCreateDto>())).Returns(voiceChannel);
            _mockVoiceChannelService.Setup(s => s.CreateVoiceChannelAsync(It.IsAny<VoiceChannel>()))
                .ReturnsAsync((voiceChannel, "Voice channel created successfully"));
            _mockMapper.Setup(m => m.Map<VoiceChannelReadDto>(It.IsAny<VoiceChannel>())).Returns(voiceChannelReadDto);

            // Act
            var result = await _controller.CreateVoiceChannel(voiceChannelCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(voiceChannelReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task CreateVoiceChannel_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var voiceChannelCreateDto = new VoiceChannelCreateDto(); // Invalid data
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreateVoiceChannel(voiceChannelCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            
            // Verify the returned error DTO
            var errorDto = badRequestResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("Invalid voice channel data.", errorDto.Message);
        }

        [TestMethod]
        public async Task CreateVoiceChannel_ServiceReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var voiceChannelCreateDto = new VoiceChannelCreateDto
            {
                Name = "Test Voice Channel",
                ClanId = Guid.NewGuid()
            };

            var voiceChannel = new VoiceChannel
            {
                Name = "Test Voice Channel",
                ClanId = voiceChannelCreateDto.ClanId,
                IsActive = true,
                MaxParticipants = 5
            };

            // Setup mocks
            _mockMapper.Setup(m => m.Map<VoiceChannel>(It.IsAny<VoiceChannelCreateDto>())).Returns(voiceChannel);
            _mockVoiceChannelService.Setup(s => s.CreateVoiceChannelAsync(It.IsAny<VoiceChannel>()))
                .ReturnsAsync((null, "Clan not found"));

            // Act
            var result = await _controller.CreateVoiceChannel(voiceChannelCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            
            // Verify the error message
            var errorDto = notFoundResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("Clan not found", errorDto.Message);
        }

        [TestMethod]
        public async Task GetVoiceChannelById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var voiceChannelId = Guid.NewGuid();
            var voiceChannel = new VoiceChannel
            {
                VoiceChannelId = voiceChannelId,
                Name = "Test Voice Channel",
                ClanId = Guid.NewGuid(),
                IsActive = true,
                MaxParticipants = 5
            };
            
            var voiceChannelReadDto = new VoiceChannelReadDto
            {
                VoiceChannelId = voiceChannel.VoiceChannelId,
                Name = voiceChannel.Name,
                ClanId = voiceChannel.ClanId,
                IsActive = voiceChannel.IsActive
            };

            _mockVoiceChannelService.Setup(s => s.GetVoiceChannelByIdAsync(voiceChannelId)).ReturnsAsync(voiceChannel);
            _mockMapper.Setup(m => m.Map<VoiceChannelReadDto>(It.IsAny<VoiceChannel>())).Returns(voiceChannelReadDto);

            // Act
            var result = await _controller.GetVoiceChannelById(voiceChannelId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(voiceChannelReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetVoiceChannelById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var voiceChannelId = Guid.NewGuid();
            _mockVoiceChannelService.Setup(s => s.GetVoiceChannelByIdAsync(voiceChannelId)).ReturnsAsync((VoiceChannel)null);

            // Act
            var result = await _controller.GetVoiceChannelById(voiceChannelId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            
            // Verify error message
            var errorDto = notFoundResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("VoiceChannel not found.", errorDto.Message);
        }

        [TestMethod]
        public async Task GetVoiceChannelsByClanId_ExistingClanId_ReturnsOkResult()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var voiceChannels = new List<VoiceChannel>
            {
                new VoiceChannel { 
                    VoiceChannelId = Guid.NewGuid(), 
                    Name = "Voice Channel 1", 
                    ClanId = clanId, 
                    IsActive = true, 
                    MaxParticipants = 5 
                },
                new VoiceChannel { 
                    VoiceChannelId = Guid.NewGuid(), 
                    Name = "Voice Channel 2", 
                    ClanId = clanId, 
                    IsActive = true, 
                    MaxParticipants = 10 
                }
            };

            var voiceChannelDtos = new List<VoiceChannelReadDto>
            {
                new VoiceChannelReadDto { 
                    VoiceChannelId = voiceChannels[0].VoiceChannelId, 
                    Name = voiceChannels[0].Name, 
                    ClanId = voiceChannels[0].ClanId, 
                    IsActive = voiceChannels[0].IsActive 
                },
                new VoiceChannelReadDto { 
                    VoiceChannelId = voiceChannels[1].VoiceChannelId, 
                    Name = voiceChannels[1].Name, 
                    ClanId = voiceChannels[1].ClanId, 
                    IsActive = voiceChannels[1].IsActive 
                }
            };

            _mockVoiceChannelService.Setup(s => s.GetVoiceChannelsByClanIdAsync(clanId)).ReturnsAsync(voiceChannels);
            _mockMapper.Setup(m => m.Map<List<VoiceChannelReadDto>>(It.IsAny<List<VoiceChannel>>())).Returns(voiceChannelDtos);

            // Act
            var result = await _controller.GetVoiceChannelsByClanId(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(voiceChannelDtos, okResult.Value);
        }

        [TestMethod]
        public async Task GetVoiceChannelsByClanId_EmptyResult_ReturnsOkWithEmptyList()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var emptyList = new List<VoiceChannel>();
            var emptyDtoList = new List<VoiceChannelReadDto>();

            _mockVoiceChannelService.Setup(s => s.GetVoiceChannelsByClanIdAsync(clanId)).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<List<VoiceChannelReadDto>>(It.IsAny<List<VoiceChannel>>())).Returns(emptyDtoList);

            // Act
            var result = await _controller.GetVoiceChannelsByClanId(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            
            var returnedDtos = okResult.Value as List<VoiceChannelReadDto>;
            Assert.IsNotNull(returnedDtos);
            Assert.AreEqual(0, returnedDtos.Count);
        }

        [TestMethod]
        public async Task UpdateVoiceChannel_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var voiceChannelUpdateDto = new VoiceChannelUpdateDto
            {
                VoiceChannelId = Guid.NewGuid(),
                Name = "Updated Voice Channel",
                IsActive = false
            };

            var existingVoiceChannel = new VoiceChannel
            {
                VoiceChannelId = voiceChannelUpdateDto.VoiceChannelId,
                Name = "Original Voice Channel",
                ClanId = Guid.NewGuid(),
                IsActive = true,
                MaxParticipants = 5
            };

            var updatedVoiceChannel = new VoiceChannel
            {
                VoiceChannelId = existingVoiceChannel.VoiceChannelId,
                Name = voiceChannelUpdateDto.Name,
                ClanId = existingVoiceChannel.ClanId,
                IsActive = voiceChannelUpdateDto.IsActive,
                MaxParticipants = existingVoiceChannel.MaxParticipants
            };

            var voiceChannelReadDto = new VoiceChannelReadDto
            {
                VoiceChannelId = updatedVoiceChannel.VoiceChannelId,
                Name = updatedVoiceChannel.Name,
                ClanId = updatedVoiceChannel.ClanId,
                IsActive = updatedVoiceChannel.IsActive
            };

            _mockVoiceChannelService.Setup(s => s.GetVoiceChannelByIdAsync(voiceChannelUpdateDto.VoiceChannelId)).ReturnsAsync(existingVoiceChannel);
            _mockVoiceChannelService.Setup(s => s.UpdateVoiceChannelAsync(It.IsAny<VoiceChannel>())).ReturnsAsync(updatedVoiceChannel);
            _mockMapper.Setup(m => m.Map<VoiceChannelReadDto>(It.IsAny<VoiceChannel>())).Returns(voiceChannelReadDto);

            // Act
            var result = await _controller.UpdateVoiceChannel(voiceChannelUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(voiceChannelReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateVoiceChannel_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var voiceChannelUpdateDto = new VoiceChannelUpdateDto(); // Invalid data
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.UpdateVoiceChannel(voiceChannelUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateVoiceChannel_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var voiceChannelUpdateDto = new VoiceChannelUpdateDto
            {
                VoiceChannelId = Guid.NewGuid(),
                Name = "Updated Voice Channel",
                IsActive = false
            };

            _mockVoiceChannelService.Setup(s => s.GetVoiceChannelByIdAsync(voiceChannelUpdateDto.VoiceChannelId)).ReturnsAsync((VoiceChannel)null);

            // Act
            var result = await _controller.UpdateVoiceChannel(voiceChannelUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteVoiceChannel_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var voiceChannelId = Guid.NewGuid();
            _mockVoiceChannelService.Setup(s => s.DeleteVoiceChannelAsync(voiceChannelId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteVoiceChannel(voiceChannelId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteVoiceChannel_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var voiceChannelId = Guid.NewGuid();
            _mockVoiceChannelService.Setup(s => s.DeleteVoiceChannelAsync(voiceChannelId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteVoiceChannel(voiceChannelId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            
            // Verify error message
            var errorDto = notFoundResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("VoiceChannel not found or already deleted.", errorDto.Message);
        }
    }
}