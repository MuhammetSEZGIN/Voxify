using System;
using Moq;
using Xunit;
using ClanService.Interfaces;
using AutoMapper;
using ClanService.Controllers;
using Microsoft.AspNetCore.Http;
using ClanService.DTOs;
using ClanService.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ClanService.Mapping;
namespace ClanServiceTest.UnitTests.Controllers;

public class ChannelControllerTest
{
    private readonly Mock<IChannelService> _channelServiceMock;
    private readonly IMapper _mapper;
    private readonly ChannelController _controller;

    public ChannelControllerTest()
    {
        _channelServiceMock = new Mock<IChannelService>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        _controller = new ChannelController(_channelServiceMock.Object, _mapper);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5000);
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    #region CreateChannel Tests

    [Fact]
    public async Task CreateChanell_Returns_BasRequest_When_ModelState_InValid()
    {
        //Arrange
        _controller.ModelState.AddModelError("Name", "Required");
        var dto = new ChannelCreateDto
        {
            Name = "",
            ClanId = Guid.NewGuid()
        };

        // Act
        var result = await _controller.CreateChannel(dto);

        //Result
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(badRequest.Value);
        Assert.Equal("Invalid channel data.", error.Message);
        Assert.NotNull(error.Errors);
    }

    [Fact]
    public async Task CreateChannel_Returns_NotFound_When_Service_Returns_Null_Channel()
    {
        // Arrange
        var dto = new ChannelCreateDto
        {
            Name = "Test Channel",
            ClanId = Guid.NewGuid()
        };

        _channelServiceMock
            .Setup(s => s.CreateChannelAsync(It.IsAny<ClanService.Models.Channel>()))
            .ReturnsAsync((null!, "Clan not found."));

        // Act
        var result = await _controller.CreateChannel(dto);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Clan not found.", error.Message);
    }



    [Fact]
    public async Task CreateChannel_Returns_OkResult_When_Channel_Created_Successfully()
    {
        // Arrange
        var dto = new ChannelCreateDto
        {
            Name = "Test Channel",
            ClanId = Guid.NewGuid()
        };
        var createdChannel = new ClanService.Models.Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = dto.Name,
            ClanId = dto.ClanId
        };

        _channelServiceMock.Setup(s => s.CreateChannelAsync(
            It.Is<ClanService.Models.Channel>(
                c => c.Name == dto.Name && c.ClanId == dto.ClanId)))
            .ReturnsAsync((createdChannel, "Channel created successfully"));

        // Act
        var result = await _controller.CreateChannel(dto);
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnDto = Assert.IsType<ChannelReadDto>(okResult.Value);

        Assert.Equal(createdChannel.ChannelId, returnDto.ChannelId);
        Assert.Equal(createdChannel.Name, returnDto.Name);
        Assert.Equal(createdChannel.ClanId, returnDto.ClanId);
    }
    #endregion

    #region GetChannelById

    [Fact]
    public async Task GetChannelById_Returns_NotFound_When_Channel_Does_Not_Exist()
    {
        // Arrange
        var channelId = Guid.NewGuid();

        _channelServiceMock
            .Setup(s => s.GetChannelByIdAsync(channelId))
            .ReturnsAsync((ClanService.Models.Channel?)null);

        // Act
        var result = await _controller.GetChannelById(channelId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Channel not found.", error.Message);
    }

    [Fact]
    public async Task GetChannelById_Returns_Ok_With_ReadDto_When_Channel_Exists()
    {
        // Arrange
        var channel = new ClanService.Models.Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "General",
            ClanId = Guid.NewGuid()
        };

        _channelServiceMock
            .Setup(s => s.GetChannelByIdAsync(channel.ChannelId))
            .ReturnsAsync(channel);

        // Act
        var result = await _controller.GetChannelById(channel.ChannelId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var readDto = Assert.IsType<ChannelReadDto>(ok.Value);

        Assert.Equal(channel.ChannelId, readDto.ChannelId);
        Assert.Equal(channel.Name, readDto.Name);
        Assert.Equal(channel.ClanId, readDto.ClanId);
    }

    #endregion

    #region GetChannelsByClanId

    [Fact]
    public async Task GetChannelsByClanId_Returns_NotFound_When_Service_Returns_Null()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        _channelServiceMock
            .Setup(s => s.GetChannelsByClanIdAsync(clanId))
            .ReturnsAsync((List<ClanService.Models.Channel>?)null);

        // Act
        var result = await _controller.GetChannelsByClanId(clanId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Channels not found.", error.Message);
    }

    [Fact]
    public async Task GetChannelsByClanId_Returns_Ok_With_Empty_List_When_No_Channels()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        _channelServiceMock
            .Setup(s => s.GetChannelsByClanIdAsync(clanId))
            .ReturnsAsync(new List<ClanService.Models.Channel>());

        // Act
        var result = await _controller.GetChannelsByClanId(clanId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<ChannelReadDto>>(ok.Value);
        Assert.Empty(list);
    }

    [Fact]
    public async Task GetChannelsByClanId_Returns_Ok_With_List_When_Channels_Exist()
    {
        // Arrange
        var clanId = Guid.NewGuid();
        var channels = new List<ClanService.Models.Channel>
        {
            new() { ChannelId = Guid.NewGuid(), Name = "General", ClanId = clanId },
            new() { ChannelId = Guid.NewGuid(), Name = "Gaming", ClanId = clanId }
        };

        _channelServiceMock
            .Setup(s => s.GetChannelsByClanIdAsync(clanId))
            .ReturnsAsync(channels);

        // Act
        var result = await _controller.GetChannelsByClanId(clanId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<ChannelReadDto>>(ok.Value);

        Assert.Equal(2, list.Count);
        Assert.All(list, dto => Assert.Equal(clanId, dto.ClanId));
        Assert.Contains(list, dto => dto.Name == "General");
        Assert.Contains(list, dto => dto.Name == "Gaming");
    }

    #endregion

    #region UpdateChannel

    [Fact]
    public async Task UpdateChannel_Returns_BadRequest_When_ModelState_Invalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        var dto = new ChannelUpdateDto
        {
            ChannelId = Guid.NewGuid(),
            Name = ""
        };

        // Act
        var result = await _controller.UpdateChannel(dto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(badRequest.Value);
        Assert.Equal("Invalid channel data.", error.Message);
        Assert.NotNull(error.Errors);
    }

    [Fact]
    public async Task UpdateChannel_Returns_NotFound_When_Channel_Does_Not_Exist()
    {
        // Arrange
        var dto = new ChannelUpdateDto
        {
            ChannelId = Guid.NewGuid(),
            Name = "New Name"
        };

        _channelServiceMock
            .Setup(s => s.GetChannelByIdAsync(dto.ChannelId))
            .ReturnsAsync((ClanService.Models.Channel?)null);

        // Act
        var result = await _controller.UpdateChannel(dto);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Channel not found.", notFound.Value);
    }

    [Fact]
    public async Task UpdateChannel_Returns_Ok_With_ReadDto_When_Updated()
    {
        // Arrange
        var existing = new ClanService.Models.Channel
        {
            ChannelId = Guid.NewGuid(),
            Name = "Old Name",
            ClanId = Guid.NewGuid()
        };

        var dto = new ChannelUpdateDto
        {
            ChannelId = existing.ChannelId,
            Name = "New Name"
        };

        _channelServiceMock
            .Setup(s => s.GetChannelByIdAsync(dto.ChannelId))
            .ReturnsAsync(existing);

        _channelServiceMock
            .Setup(s => s.UpdateChannelAsync(It.IsAny<ClanService.Models.Channel>()))
            .ReturnsAsync((ClanService.Models.Channel c) => c); // return what controller sends

        // Act
        var result = await _controller.UpdateChannel(dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var readDto = Assert.IsType<ChannelReadDto>(ok.Value);

        Assert.Equal(existing.ChannelId, readDto.ChannelId);
        Assert.Equal(dto.Name, readDto.Name);
        Assert.Equal(existing.ClanId, readDto.ClanId);
    }

    #endregion

    #region DeleteChannel

    [Fact]
    public async Task DeleteChannel_Returns_NotFound_When_Service_Returns_False()
    {
        // Arrange
        var channelId = Guid.NewGuid();

        _channelServiceMock
            .Setup(s => s.DeleteChannelAsync(channelId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteChannel(channelId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Channel not found or already deleted.", error.Message);
    }

    [Fact]
    public async Task DeleteChannel_Returns_NoContent_When_Deleted()
    {
        // Arrange
        var channelId = Guid.NewGuid();

        _channelServiceMock
            .Setup(s => s.DeleteChannelAsync(channelId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteChannel(channelId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    #endregion
}