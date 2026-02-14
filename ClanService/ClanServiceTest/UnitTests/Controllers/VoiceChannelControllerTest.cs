using System;
using System.Collections.Generic;
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

public class VoiceChannelControllerTest
{
    private readonly Mock<IVoiceChannelService> _voiceChannelServiceMock;
    private readonly IMapper _mapper;
    private readonly VoiceChannelController _controller;

    public VoiceChannelControllerTest()
    {
        _voiceChannelServiceMock = new Mock<IVoiceChannelService>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        _controller = new VoiceChannelController(_voiceChannelServiceMock.Object, _mapper);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5000);
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    #region CreateVoiceChannel

    [Fact]
    public async Task CreateVoiceChannel_Returns_BadRequest_When_ModelState_Invalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        var dto = new VoiceChannelCreateDto
        {
            Name = "",
            ClanId = Guid.NewGuid()
        };

        // Act
        var result = await _controller.CreateVoiceChannel(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(badRequest.Value);
        Assert.Equal("Invalid voice channel data.", error.Message);
        Assert.NotNull(error.Errors);
    }

    [Fact]
    public async Task CreateVoiceChannel_Returns_NotFound_When_Service_Returns_Null_Channel()
    {
        // Arrange
        var dto = new VoiceChannelCreateDto
        {
            Name = "VC",
            ClanId = Guid.NewGuid()
        };

        _voiceChannelServiceMock
            .Setup(s => s.CreateVoiceChannelAsync(It.IsAny<VoiceChannel>()))
            .ReturnsAsync((null!, "Clan not found."));

        // Act
        var result = await _controller.CreateVoiceChannel(dto);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Clan not found.", error.Message);
    }

    [Fact]
    public async Task CreateVoiceChannel_Returns_Ok_With_ReadDto_When_Created()
    {
        // Arrange
        var clanId = Guid.NewGuid();
        var dto = new VoiceChannelCreateDto
        {
            Name = "General Voice",
            ClanId = clanId
        };

        var created = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = dto.Name,
            ClanId = dto.ClanId,
            IsActive = true
        };

        _voiceChannelServiceMock
            .Setup(s => s.CreateVoiceChannelAsync(It.IsAny<VoiceChannel>()))
            .ReturnsAsync((created, "ok"));

        // Act
        var result = await _controller.CreateVoiceChannel(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var readDto = Assert.IsType<VoiceChannelReadDto>(ok.Value);
        Assert.Equal(created.VoiceChannelId, readDto.VoiceChannelId);
        Assert.Equal(created.Name, readDto.Name);
        Assert.Equal(created.ClanId, readDto.ClanId);
        Assert.Equal(created.IsActive, readDto.IsActive);
    }

    #endregion

    #region GetVoiceChannelById

    [Fact]
    public async Task GetVoiceChannelById_Returns_NotFound_When_Channel_Does_Not_Exist()
    {
        // Arrange
        var id = Guid.NewGuid();

        _voiceChannelServiceMock
            .Setup(s => s.GetVoiceChannelByIdAsync(id))
            .ReturnsAsync((VoiceChannel?)null);

        // Act
        var result = await _controller.GetVoiceChannelById(id);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("VoiceChannel not found.", error.Message);
    }

    [Fact]
    public async Task GetVoiceChannelById_Returns_Ok_With_ReadDto_When_Found()
    {
        // Arrange
        var channel = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "VC",
            ClanId = Guid.NewGuid(),
            IsActive = true
        };

        _voiceChannelServiceMock
            .Setup(s => s.GetVoiceChannelByIdAsync(channel.VoiceChannelId))
            .ReturnsAsync(channel);

        // Act
        var result = await _controller.GetVoiceChannelById(channel.VoiceChannelId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<VoiceChannelReadDto>(ok.Value);
        Assert.Equal(channel.VoiceChannelId, dto.VoiceChannelId);
        Assert.Equal(channel.Name, dto.Name);
        Assert.Equal(channel.ClanId, dto.ClanId);
        Assert.Equal(channel.IsActive, dto.IsActive);
    }

    #endregion

    #region GetVoiceChannelsByClanId

    [Fact]
    public async Task GetVoiceChannelsByClanId_Returns_Ok_With_Empty_List_When_None()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        _voiceChannelServiceMock
            .Setup(s => s.GetVoiceChannelsByClanIdAsync(clanId))
            .ReturnsAsync(new List<VoiceChannel>());

        // Act
        var result = await _controller.GetVoiceChannelsByClanId(clanId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<VoiceChannelReadDto>>(ok.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task GetVoiceChannelsByClanId_Returns_Ok_With_List_When_Exists()
    {
        // Arrange
        var clanId = Guid.NewGuid();
        var channels = new List<VoiceChannel>
        {
            new() { VoiceChannelId = Guid.NewGuid(), Name = "A", ClanId = clanId, IsActive = true },
            new() { VoiceChannelId = Guid.NewGuid(), Name = "B", ClanId = clanId, IsActive = false }
        };

        _voiceChannelServiceMock
            .Setup(s => s.GetVoiceChannelsByClanIdAsync(clanId))
            .ReturnsAsync(channels);

        // Act
        var result = await _controller.GetVoiceChannelsByClanId(clanId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<VoiceChannelReadDto>>(ok.Value);
        Assert.Equal(2, list.Count);
        Assert.All(list, x => Assert.Equal(clanId, x.ClanId));
    }

    #endregion

    #region UpdateVoiceChannel

    [Fact]
    public async Task UpdateVoiceChannel_Returns_BadRequest_When_ModelState_Invalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        var dto = new VoiceChannelUpdateDto
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "",
            IsActive = true
        };

        // Act
        var result = await _controller.UpdateVoiceChannel(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(badRequest.Value);
        Assert.Equal("Invalid voice channel data.", error.Message);
        Assert.NotNull(error.Errors);
    }

    [Fact]
    public async Task UpdateVoiceChannel_Returns_NotFound_When_Channel_Does_Not_Exist()
    {
        // Arrange
        var dto = new VoiceChannelUpdateDto
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "New",
            IsActive = false
        };

        _voiceChannelServiceMock
            .Setup(s => s.GetVoiceChannelByIdAsync(dto.VoiceChannelId))
            .ReturnsAsync((VoiceChannel?)null);

        // Act
        var result = await _controller.UpdateVoiceChannel(dto);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("VoiceChannel not found.", error.Message);
    }

    [Fact]
    public async Task UpdateVoiceChannel_Returns_Ok_With_ReadDto_When_Updated()
    {
        // Arrange
        var existing = new VoiceChannel
        {
            VoiceChannelId = Guid.NewGuid(),
            Name = "Old",
            ClanId = Guid.NewGuid(),
            IsActive = true
        };

        var dto = new VoiceChannelUpdateDto
        {
            VoiceChannelId = existing.VoiceChannelId,
            Name = "New",
            IsActive = false
        };

        _voiceChannelServiceMock
            .Setup(s => s.GetVoiceChannelByIdAsync(dto.VoiceChannelId))
            .ReturnsAsync(existing);

        _voiceChannelServiceMock
            .Setup(s => s.UpdateVoiceChannelAsync(It.IsAny<VoiceChannel>()))
            .ReturnsAsync((VoiceChannel vc) => vc);

        // Act
        var result = await _controller.UpdateVoiceChannel(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var readDto = Assert.IsType<VoiceChannelReadDto>(ok.Value);
        Assert.Equal(existing.VoiceChannelId, readDto.VoiceChannelId);
        Assert.Equal("New", readDto.Name);
        Assert.Equal(existing.ClanId, readDto.ClanId);
        Assert.False(readDto.IsActive);
    }

    #endregion

    #region DeleteVoiceChannel

    [Fact]
    public async Task DeleteVoiceChannel_Returns_NotFound_When_Service_Returns_False()
    {
        // Arrange
        var id = Guid.NewGuid();

        _voiceChannelServiceMock
            .Setup(s => s.DeleteVoiceChannelAsync(id))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteVoiceChannel(id);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("VoiceChannel not found or already deleted.", error.Message);
    }

    [Fact]
    public async Task DeleteVoiceChannel_Returns_NoContent_When_Deleted()
    {
        // Arrange
        var id = Guid.NewGuid();

        _voiceChannelServiceMock
            .Setup(s => s.DeleteVoiceChannelAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteVoiceChannel(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    #endregion
}