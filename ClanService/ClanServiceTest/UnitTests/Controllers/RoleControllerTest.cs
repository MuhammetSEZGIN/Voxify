using System;
using System.Threading.Tasks;
using ClanService.Controllers;
using ClanService.DTOs.ClanMembershipDtos;
using ClanService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ClanServiceTest.UnitTests.Controllers;

public class RoleControllerTest
{
    private readonly Mock<IRoleService> _roleServiceMock;
    private readonly RoleController _controller;

    public RoleControllerTest()
    {
        _roleServiceMock = new Mock<IRoleService>();
        _controller = new RoleController(_roleServiceMock.Object);
    }

    [Fact]
    public async Task UpdateRoleAsync_Returns_BadRequest_When_Dto_Is_Null()
    {
        // Act
        var result = await _controller.UpdateRoleAsync(null!);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid input data.", bad.Value);
        _roleServiceMock.Verify(s => s.UpdateRoleAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoleAsync_Returns_BadRequest_When_MembershipId_Empty()
    {
        // Arrange
        var dto = new UpdateRoleDto
        {
            MembershipId = Guid.Empty,
            RoleName = "Admin"
        };

        // Act
        var result = await _controller.UpdateRoleAsync(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid input data.", bad.Value);
        _roleServiceMock.Verify(s => s.UpdateRoleAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateRoleAsync_Returns_BadRequest_When_RoleName_NullOrEmpty(string roleName)
    {
        // Arrange
        var dto = new UpdateRoleDto
        {
            MembershipId = Guid.NewGuid(),
            RoleName = roleName!
        };

        // Act
        var result = await _controller.UpdateRoleAsync(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid input data.", bad.Value);
        _roleServiceMock.Verify(s => s.UpdateRoleAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoleAsync_Returns_BadRequest_When_Service_Fails()
    {
        // Arrange
        var dto = new UpdateRoleDto
        {
            MembershipId = Guid.NewGuid(),
            RoleName = "Admin"
        };

        _roleServiceMock
            .Setup(s => s.UpdateRoleAsync(dto.MembershipId, dto.RoleName))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateRoleAsync(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to update role.", bad.Value);
        _roleServiceMock.Verify(s => s.UpdateRoleAsync(dto.MembershipId, dto.RoleName), Times.Once);
    }

    [Fact]
    public async Task UpdateRoleAsync_Returns_Ok_When_Service_Succeeds()
    {
        // Arrange
        var dto = new UpdateRoleDto
        {
            MembershipId = Guid.NewGuid(),
            RoleName = "Admin"
        };

        _roleServiceMock
            .Setup(s => s.UpdateRoleAsync(dto.MembershipId, dto.RoleName))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateRoleAsync(dto);

        // Assert
        Assert.IsType<OkResult>(result);
        _roleServiceMock.Verify(s => s.UpdateRoleAsync(dto.MembershipId, dto.RoleName), Times.Once);
    }
}