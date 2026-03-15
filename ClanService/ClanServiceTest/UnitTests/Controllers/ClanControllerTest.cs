using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClanService.Controllers;
using ClanService.DTOs;
using ClanService.Interfaces;
using ClanService.Mapping;
using ClanService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ClanServiceTest.UnitTests.Controllers;

public class ClanControllerTest
{
    private readonly Mock<IClanService> _clanServiceMock;
    private readonly IMapper _mapper;
    private readonly ClanController _controller;

    public ClanControllerTest()
    {
        _clanServiceMock = new Mock<IClanService>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        _controller = new ClanController(_clanServiceMock.Object, _mapper);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5000);
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    #region CreateClan

    [Fact]
    public async Task CreateClan_Returns_BadRequest_When_ModelState_Invalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        var dto = new ClanCreateDto
        {
            Name = "",
            UserId = "user-1",
            ImagePath = "img.png",
            Description = "desc"
        };

        // Act
        var result = await _controller.CreateClan(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(badRequest.Value);
        Assert.Equal("Invalid clan data.", error.Message);
        Assert.NotNull(error.Errors);
    }

    [Fact]
    public async Task CreateClan_Returns_BadRequest_When_Service_Returns_Null_Created()
    {
        // Arrange
        var dto = new ClanCreateDto
        {
            Name = "Test Clan",
            UserId = "user-1",
            ImagePath = "img.png",
            Description = "desc"
        };

        _clanServiceMock
            .Setup(s => s.CreateClanAsync(It.IsAny<Clan>(), dto.UserId))
            .ReturnsAsync((null!, "Cannot create clan."));

        // Act
        var result = await _controller.CreateClan(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(badRequest.Value);
        Assert.Equal("Cannot create clan.", error.Message);
    }

    [Fact]
    public async Task CreateClan_Returns_Ok_With_ClanReadDto_When_Created()
    {
        // Arrange
        var dto = new ClanCreateDto
        {
            Name = "Test Clan",
            UserId = "user-1",
            ImagePath = "img.png",
            Description = "desc"
        };

        var created = new Clan
        {
            ClanId = Guid.NewGuid(),
            Name = dto.Name,
            ImagePath = dto.ImagePath,
            Description = dto.Description
        };

        _clanServiceMock
            .Setup(s => s.CreateClanAsync(It.IsAny<Clan>(), dto.UserId))
            .ReturnsAsync((created, "ok"));

        // Act
        var result = await _controller.CreateClan(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var readDto = Assert.IsType<ClanReadDto>(ok.Value);
        Assert.Equal(created.ClanId, readDto.ClanId);
        Assert.Equal(created.Name, readDto.Name);
        Assert.Equal(created.ImagePath, readDto.ImagePath);
        Assert.Equal(created.Description, readDto.Description);
    }

    #endregion

    #region GetClanById

    [Fact]
    public async Task GetClanById_Returns_NotFound_When_Clan_Does_Not_Exist()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        _clanServiceMock
            .Setup(s => s.GetClanByIdAsync(clanId))
            .ReturnsAsync((Clan?)null);

        // Act
        var result = await _controller.GetClanById(clanId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Clan not found.", error.Message);
    }

    [Fact]
    public async Task GetClanById_Returns_Ok_With_GetAllClanPropertyDto_When_Found()
    {
        // Arrange
        var clan = new Clan
        {
            ClanId = Guid.NewGuid(),
            Name = "Clan",
            ImagePath = "img.png",
            Description = "desc",
            Channels = new List<Channel>(),
            VoiceChannels = new List<VoiceChannel>(),
            ClanMemberShips = new List<ClanMembership>()
        };

        _clanServiceMock
            .Setup(s => s.GetClanByIdAsync(clan.ClanId))
            .ReturnsAsync(clan);

        // Act
        var result = await _controller.GetClanById(clan.ClanId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<GetAllClanPropertyDto>(ok.Value);
        Assert.Equal(clan.ClanId, dto.ClanId);
        Assert.Equal(clan.Name, dto.Name);
        Assert.Equal(clan.ImagePath, dto.ImagePath);
        Assert.Equal(clan.Description, dto.Description);
        Assert.NotNull(dto.Channels);
        Assert.NotNull(dto.VoiceChannels);
        Assert.NotNull(dto.ClanMemberships);
    }

    #endregion

    #region GetAllClans

    [Fact]
    public async Task GetAllClans_Returns_Ok_With_List()
    {
        // Arrange
        var clans = new List<Clan>
        {
            new() { ClanId = Guid.NewGuid(), Name = "A", ImagePath = "a.png", Description = "da" },
            new() { ClanId = Guid.NewGuid(), Name = "B", ImagePath = "b.png", Description = "db" }
        };

        _clanServiceMock
            .Setup(s => s.GetAllClansAsync())
            .ReturnsAsync(clans);

        // Act
        var result = await _controller.GetAllClans();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<ClanReadDto>>(ok.Value);
        Assert.Equal(2, list.Count);
        Assert.Contains(list, x => x.Name == "A");
        Assert.Contains(list, x => x.Name == "B");
    }

    #endregion

    #region UpdateClan

    [Fact]
    public async Task UpdateClan_Returns_BadRequest_When_ModelState_Invalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        var dto = new ClanUpdateDto
        {
            ClanId = Guid.NewGuid(),
            Name = "",
            ImagePath = "img.png"
        };

        // Act
        var result = await _controller.UpdateClan(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(badRequest.Value);
        Assert.Equal("Invalid clan data.", error.Message);
        Assert.NotNull(error.Errors);
    }

    [Fact]
    public async Task UpdateClan_Returns_NotFound_When_Clan_Does_Not_Exist()
    {
        // Arrange
        var dto = new ClanUpdateDto
        {
            ClanId = Guid.NewGuid(),
            Name = "New Name",
            ImagePath = "new.png"
        };

        _clanServiceMock
            .Setup(s => s.GetClanByIdAsync(dto.ClanId))
            .ReturnsAsync((Clan?)null);

        // Act
        var result = await _controller.UpdateClan(dto);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Clan not found.", error.Message);
    }

    [Fact]
    public async Task UpdateClan_Returns_Ok_With_ClanReadDto_When_Updated()
    {
        // Arrange
        var existing = new Clan
        {
            ClanId = Guid.NewGuid(),
            Name = "Old",
            ImagePath = "old.png",
            Description = "desc"
        };

        var dto = new ClanUpdateDto
        {
            ClanId = existing.ClanId,
            Name = "New",
            ImagePath = "new.png"
        };

        _clanServiceMock
            .Setup(s => s.GetClanByIdAsync(dto.ClanId))
            .ReturnsAsync(existing);

        _clanServiceMock
            .Setup(s => s.UpdateClanAsync(It.IsAny<Clan>()))
            .ReturnsAsync((Clan c) => c);

        // Act
        var result = await _controller.UpdateClan(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var readDto = Assert.IsType<ClanReadDto>(ok.Value);

        Assert.Equal(existing.ClanId, readDto.ClanId);
        Assert.Equal("New", readDto.Name);
        Assert.Equal("new.png", readDto.ImagePath);
    }

    #endregion

    #region DeleteClan

    [Fact]
    public async Task DeleteClan_Returns_NotFound_When_Service_Returns_False()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        _clanServiceMock
            .Setup(s => s.DeleteClanAsync(clanId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteClan(clanId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Clan not found or already deleted.", error.Message);
    }

    [Fact]
    public async Task DeleteClan_Returns_Ok_When_Deleted()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        _clanServiceMock
            .Setup(s => s.DeleteClanAsync(clanId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteClan(clanId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    #endregion

    #region GetClansByUserIdAsync

    [Fact]
    public async Task GetClansByUserIdAsync_Returns_Ok_With_List()
    {
        // Arrange
        var userId = "user-1";
        var clans = new List<Clan>
        {
            new() { ClanId = Guid.NewGuid(), Name = "A", ImagePath = "a.png", Description = "da" }
        };

        _clanServiceMock
            .Setup(s => s.GetClansByUserIdAsync(userId))
            .ReturnsAsync(clans);

        // Act
        var result = await _controller.GetClansByUserIdAsync(userId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<ClanReadDto>>(ok.Value);
        Assert.Single(list);
        Assert.Equal("A", list[0].Name);
    }

    #endregion
}