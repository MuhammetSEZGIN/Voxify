using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ClanService.Controllers;
using ClanService.DTOs;
using ClanService.Interfaces;
using ClanService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClanService.Controllers.Tests
{
    [TestClass]
    public class ClanControllerTest
    {
        private Mock<IClanService> _mockClanService;
        private Mock<IMapper> _mockMapper;
        private ClanController _controller;
        private Mock<HttpContext> _mockHttpContext;
        
        [TestInitialize]
        public void Setup()
        {
            _mockClanService = new Mock<IClanService>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContext = new Mock<HttpContext>();

            var items = new Dictionary<object, object>
            {
                ["UserId"] = "testUserId"
            };
            _mockHttpContext.Setup(x => x.Items).Returns(items);

            _controller = new ClanController(_mockClanService.Object, _mockMapper.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };
        }

        [TestMethod]
        public async Task CreateClan_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var clanCreateDto = new ClanCreateDto
            {
                Name = "Test Clan",
                Description = "Test Description",
                ImagePath = "test/image.jpg",
                UserId = "testUserId"
            };

            var clan = new Clan
            {
                ClanId = Guid.NewGuid(),
                Name = "Test Clan",
                Description = "Test Description",
                ImagePath = "test/image.jpg"
            };

            var clanReadDto = new ClanReadDto
            {
                ClanId = clan.ClanId,
                Name = clan.Name,
                Description = clan.Description,
                ImagePath = clan.ImagePath
            };

            // Setup mocks
            _mockMapper.Setup(m => m.Map<Clan>(It.IsAny<ClanCreateDto>())).Returns(clan);
            _mockClanService.Setup(s => s.CreateClanAsync(It.IsAny<Clan>(), It.IsAny<string>()))
                .ReturnsAsync((clan, "Clan created successfully"));
            _mockMapper.Setup(m => m.Map<ClanReadDto>(It.IsAny<Clan>())).Returns(clanReadDto);

            // Act
            var result = await _controller.CreateClan(clanCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(clanReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task CreateClan_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var clanCreateDto = new ClanCreateDto(); // Invalid data
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreateClan(clanCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task CreateClan_ServiceReturnsNull_ReturnsBadRequest()
        {
            // Arrange
            var clanCreateDto = new ClanCreateDto
            {
                Name = "Test Clan",
                Description = "Test Description",
                ImagePath = "test/image.jpg",
                UserId = "testUserId"
            };

            var clan = new Clan
            {
                Name = "Test Clan",
                Description = "Test Description",
                ImagePath = "test/image.jpg"
            };

            // Setup mocks
            _mockMapper.Setup(m => m.Map<Clan>(It.IsAny<ClanCreateDto>())).Returns(clan);
            _mockClanService.Setup(s => s.CreateClanAsync(It.IsAny<Clan>(), It.IsAny<string>()))
                .ReturnsAsync((null, "User not found"));

            // Act
            var result = await _controller.CreateClan(clanCreateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            
            // Verify error message
            var errorDto = badRequestResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("User not found", errorDto.Message);
        }

        [TestMethod]
        public async Task GetClanById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var clan = new Clan
            {
                ClanId = clanId,
                Name = "Test Clan",
                Description = "Test Description",
                ImagePath = "test/image.jpg"
            };
            
            var clanDetailsDto = new GetAllClanPropertyDto
            {
                ClanId = clan.ClanId,
                Name = clan.Name,
                Description = clan.Description,
                ImagePath = clan.ImagePath,
                Channels = new List<ChannelReadDto>(),
                VoiceChannels = new List<VoiceChannelReadDto>(),
                ClanMemberships = new List<UserMembershipDto>()
            };

            _mockClanService.Setup(s => s.GetClanByIdAsync(clanId)).ReturnsAsync(clan);
            _mockMapper.Setup(m => m.Map<GetAllClanPropertyDto>(It.IsAny<Clan>())).Returns(clanDetailsDto);

            // Act
            var result = await _controller.GetClanById(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(clanDetailsDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetClanById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            _mockClanService.Setup(s => s.GetClanByIdAsync(clanId)).ReturnsAsync((Clan)null);

            // Act
            var result = await _controller.GetClanById(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            
            // Verify error message
            var errorDto = notFoundResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("Clan not found.", errorDto.Message);
        }

        [TestMethod]
        public async Task GetAllClans_ReturnsOkResult()
        {
            // Arrange
            var clans = new List<Clan>
            {
                new Clan { ClanId = Guid.NewGuid(), Name = "Clan 1", Description = "Description 1", ImagePath = "path1" },
                new Clan { ClanId = Guid.NewGuid(), Name = "Clan 2", Description = "Description 2", ImagePath = "path2" }
            };

            var clanDtos = new List<ClanReadDto>
            {
                new ClanReadDto { ClanId = clans[0].ClanId, Name = clans[0].Name, Description = clans[0].Description, ImagePath = clans[0].ImagePath },
                new ClanReadDto { ClanId = clans[1].ClanId, Name = clans[1].Name, Description = clans[1].Description, ImagePath = clans[1].ImagePath }
            };

            _mockClanService.Setup(s => s.GetAllClansAsync()).ReturnsAsync(clans);
            _mockMapper.Setup(m => m.Map<List<ClanReadDto>>(It.IsAny<List<Clan>>())).Returns(clanDtos);

            // Act
            var result = await _controller.GetAllClans();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(clanDtos, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateClan_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var clanUpdateDto = new ClanUpdateDto
            {
                ClanId = Guid.NewGuid(),
                Name = "Updated Clan",
                ImagePath = "updated/path.jpg"
            };

            var existingClan = new Clan
            {
                ClanId = clanUpdateDto.ClanId,
                Name = "Original Clan",
                Description = "Original Description",
                ImagePath = "original/path.jpg"
            };

            var updatedClan = new Clan
            {
                ClanId = existingClan.ClanId,
                Name = clanUpdateDto.Name,
                Description = existingClan.Description,
                ImagePath = clanUpdateDto.ImagePath
            };

            var clanReadDto = new ClanReadDto
            {
                ClanId = updatedClan.ClanId,
                Name = updatedClan.Name,
                Description = updatedClan.Description,
                ImagePath = updatedClan.ImagePath
            };

            _mockClanService.Setup(s => s.GetClanByIdAsync(clanUpdateDto.ClanId)).ReturnsAsync(existingClan);
            _mockClanService.Setup(s => s.UpdateClanAsync(It.IsAny<Clan>())).ReturnsAsync(updatedClan);
            _mockMapper.Setup(m => m.Map<ClanReadDto>(It.IsAny<Clan>())).Returns(clanReadDto);

            // Act
            var result = await _controller.UpdateClan(clanUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(clanReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task UpdateClan_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var clanUpdateDto = new ClanUpdateDto(); // Invalid data
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.UpdateClan(clanUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateClan_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var clanUpdateDto = new ClanUpdateDto
            {
                ClanId = Guid.NewGuid(),
                Name = "Updated Clan",
                ImagePath = "updated/path.jpg"
            };

            _mockClanService.Setup(s => s.GetClanByIdAsync(clanUpdateDto.ClanId)).ReturnsAsync((Clan)null);

            // Act
            var result = await _controller.UpdateClan(clanUpdateDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteClan_ExistingId_ReturnsOk()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            _mockClanService.Setup(s => s.DeleteClanAsync(clanId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteClan(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task DeleteClan_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            _mockClanService.Setup(s => s.DeleteClanAsync(clanId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteClan(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public async Task GetClansByUserIdAsync_ReturnsOkResult()
        {
            // Arrange
            var userId = "testUserId";
            var clans = new List<Clan>
            {
                new Clan { ClanId = Guid.NewGuid(), Name = "Clan 1", Description = "Description 1", ImagePath = "path1" },
                new Clan { ClanId = Guid.NewGuid(), Name = "Clan 2", Description = "Description 2", ImagePath = "path2" }
            };

            var clanDtos = new List<ClanReadDto>
            {
                new ClanReadDto { ClanId = clans[0].ClanId, Name = clans[0].Name, Description = clans[0].Description, ImagePath = clans[0].ImagePath },
                new ClanReadDto { ClanId = clans[1].ClanId, Name = clans[1].Name, Description = clans[1].Description, ImagePath = clans[1].ImagePath }
            };

            _mockClanService.Setup(s => s.GetClansByUserIdAsync(userId)).ReturnsAsync(clans);
            _mockMapper.Setup(m => m.Map<List<ClanReadDto>>(It.IsAny<List<Clan>>())).Returns(clanDtos);

            // Act
            var result = await _controller.GetClansByUserIdAsync(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(clanDtos, okResult.Value);
        }

        [TestMethod]
        public async Task GetClansByUserIdAsync_EmptyList_ReturnsOkWithEmptyList()
        {
            // Arrange
            var userId = "testUserId";
            var clans = new List<Clan>();
            var clanDtos = new List<ClanReadDto>();

            _mockClanService.Setup(s => s.GetClansByUserIdAsync(userId)).ReturnsAsync(clans);
            _mockMapper.Setup(m => m.Map<List<ClanReadDto>>(It.IsAny<List<Clan>>())).Returns(clanDtos);

            // Act
            var result = await _controller.GetClansByUserIdAsync(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            
            var returnedDtos = okResult.Value as List<ClanReadDto>;
            Assert.IsNotNull(returnedDtos);
            Assert.AreEqual(0, returnedDtos.Count);
        }
    }
}