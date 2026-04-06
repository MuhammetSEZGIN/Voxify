using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ClanService.Controllers;
using ClanService.DTOs;
using ClanService.DTOs.ClanDtos;
using ClanService.DTOs.ClanMembershipDtos;
using ClanService.Interfaces;
using ClanService.Mapping;
using ClanService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace ClanServiceTest.UnitTests.Controllers;

public class ClanMembershipControllerTest
{
    private readonly Mock<IClanMembershipService> _membershipServiceMock;
    private readonly Mock<IClanService> _clanServiceMock;
    private readonly IMapper _mapper;
    private readonly ClanMembershipController _controller;

    public ClanMembershipControllerTest()
    {
        _membershipServiceMock = new Mock<IClanMembershipService>();
        _clanServiceMock = new Mock<IClanService>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        }).CreateMapper();

        _controller = new ClanMembershipController(
            _membershipServiceMock.Object,
            _mapper,
            _clanServiceMock.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5000);
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    private void SetUser(string userId)
    {
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "TestAuth"));
    }

    #region GetMembership

    [Fact]
    public async Task GetMembership_Returns_NotFound_When_Membership_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        _membershipServiceMock.Setup(s => s.GetMembershipAsync(id))
            .ReturnsAsync((ClanMembership?)null);

        // Act
        var result = await _controller.GetMembership(id);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Membership not found.", error.Message);
    }

    [Fact]
    public async Task GetMembership_Returns_Ok_With_ReadDto_When_Found()
    {
        // Arrange
        var membership = new ClanMembership
        {
            Id = Guid.NewGuid(),
            ClanId = Guid.NewGuid(),
            UserId = "user-1",
            Role = "Member"
        };

        _membershipServiceMock.Setup(s => s.GetMembershipAsync(membership.Id))
            .ReturnsAsync(membership);

        // Act
        var result = await _controller.GetMembership(membership.Id);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ClanMembershipReadDto>(ok.Value);
        Assert.Equal(membership.Id, dto.Id);
        Assert.Equal(membership.ClanId, dto.ClanId);
        Assert.Equal(membership.UserId, dto.UserId);
        Assert.Equal(membership.Role, dto.Role);
    }

    #endregion

    #region GetMembershipsByClanId

    [Fact]
    public async Task GetMembershipsByClanId_Returns_Ok_With_List()
    {
        // Arrange
        var clanId = Guid.NewGuid();
        var memberships = new List<ClanMembership>
        {
            new() { Id = Guid.NewGuid(), ClanId = clanId, UserId = "user-1", Role = "Member" },
            new() { Id = Guid.NewGuid(), ClanId = clanId, UserId = "user-2", Role = "Admin" }
        };

        _membershipServiceMock.Setup(s => s.GetMembershipsByClanIdAsync(clanId))
            .ReturnsAsync(memberships);

        // Act
        var result = await _controller.GetMembershipsByClanId(clanId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<ClanMembershipReadDto>>(ok.Value);
        Assert.Equal(2, list.Count);
        Assert.All(list, x => Assert.Equal(clanId, x.ClanId));
    }

    #endregion

    #region GetMembershipsByUserId

    [Fact]
    public async Task GetMembershipsByUserId_Returns_Ok_With_List()
    {
        // Arrange
        var userId = "user-1";
        var memberships = new List<ClanMembership>
        {
            new() { Id = Guid.NewGuid(), ClanId = Guid.NewGuid(), UserId = userId, Role = "Member" }
        };

        _membershipServiceMock.Setup(s => s.GetMembershipsByUserIdAsync(userId))
            .ReturnsAsync(memberships);

        // Act
        var result = await _controller.GetMembershipsByUserId(userId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<ClanMembershipReadDto>>(ok.Value);
        Assert.Single(list);
        Assert.Equal(userId, list[0].UserId);
    }

    #endregion

    #region RemoveMember

    [Fact]
    public async Task RemoveMember_Returns_NotFound_When_Remove_Fails()
    {
        // Arrange
        var userId = "user-1";
        var clanId = Guid.NewGuid();
        _membershipServiceMock.Setup(s => s.RemoveMemberAsync(userId, clanId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.RemoveMember(userId, clanId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Membership not found or already removed.", error.Message);
    }

    [Fact]
    public async Task RemoveMember_Returns_NoContent_When_Removed()
    {
        // Arrange
        var userId = "user-1";
        var clanId = Guid.NewGuid();
        _membershipServiceMock.Setup(s => s.RemoveMemberAsync(userId, clanId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.RemoveMember(userId, clanId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    #endregion

    #region LeaveClan

    [Fact]
    public async Task LeaveClan_Returns_NotFound_When_Service_Returns_Null_Membership()
    {
        // Arrange
        var userId = "user-1";
        var clanId = Guid.NewGuid();
        SetUser(userId);

        _membershipServiceMock.Setup(s => s.LeaveClanAsync(userId, clanId))
            .ReturnsAsync((null!, "User is not a member of this clan."));

        // Act
        var result = await _controller.LeaveClan(clanId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("User is not a member of this clan.", error.Message);
    }

    [Fact]
    public async Task LeaveClan_Returns_NoContent_When_Left_Successfully()
    {
        // Arrange
        var userId = "user-1";
        var clanId = Guid.NewGuid();
        SetUser(userId);

        var membership = new ClanMembership
        {
            Id = Guid.NewGuid(),
            ClanId = clanId,
            UserId = userId,
            Role = "Member"
        };

        _membershipServiceMock.Setup(s => s.LeaveClanAsync(userId, clanId))
            .ReturnsAsync((membership, "ok"));

        // Act
        var result = await _controller.LeaveClan(clanId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    #endregion

    #region CreateInvitation

    [Fact]
    public async Task CreateInvitation_Returns_NotFound_When_Clan_Not_Found()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        _clanServiceMock.Setup(s => s.GetClanByIdAsync(clanId))
            .ReturnsAsync((Clan?)null);

        // Act
        var result = await _controller.CreateInvitation(clanId);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(notFound.Value);
        Assert.Equal("Clan not found.", error.Message);
    }

    [Fact]
    public async Task CreateInvitation_Returns_Ok_With_InviteClanDto_When_Created()
    {
        // Arrange
        var clanId = Guid.NewGuid();

        _clanServiceMock.Setup(s => s.GetClanByIdAsync(clanId))
            .ReturnsAsync(new Clan { ClanId = clanId, Name = "Clan" });

        var invitation = new ClanInvitation
        {
            InviteId = Guid.NewGuid(),
            ClanId = clanId,
            InviteCode = "CODE123",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            MaxUses = 10,
            UsedCount = 0,
            IsActive = true
        };

        _clanServiceMock.Setup(s => s.CreateInviteTokenAsync(clanId, null, null))
            .ReturnsAsync(invitation);

        // Act
        var result = await _controller.CreateInvitation(clanId);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<InviteClanDto>(ok.Value);
        Assert.Equal(invitation.InviteCode, dto.InviteCode);
        Assert.Equal(invitation.ExpiresAt, dto.ExpiresAt);
        Assert.Equal(invitation.MaxUses, dto.MaxUses);
    }

    #endregion

    #region JoinClanWithInvite

    [Fact]
    public async Task JoinClanWithInvite_Returns_BadRequest_When_Invite_Invalid()
    {
        // Arrange
        SetUser("user-1");
        var input = new InviteCodeDto { InviteCode = "BAD" };

        _clanServiceMock.Setup(s => s.ValidateAndUseInvitationAsync(input.InviteCode))
            .ReturnsAsync((false, "Invalid invite.", null!));

        // Act
        var result = await _controller.JoinClanWithInvite(input);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(bad.Value);
        Assert.Equal("Invalid invite.", error.Message);
    }

    [Fact]
    public async Task JoinClanWithInvite_Returns_BadRequest_When_AddMember_Fails()
    {
        // Arrange
        var userId = "user-1";
        SetUser(userId);
        var input = new InviteCodeDto { InviteCode = "OK" };
        var invitation = new ClanInvitation
        {
            InviteId = Guid.NewGuid(),
            ClanId = Guid.NewGuid(),
            InviteCode = "OK",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            MaxUses = 1,
            UsedCount = 0,
            IsActive = true
        };

        _clanServiceMock.Setup(s => s.ValidateAndUseInvitationAsync(input.InviteCode))
            .ReturnsAsync((true, "ok", invitation));

        _membershipServiceMock.Setup(s => s.AddMemberAsync(It.IsAny<ClanMembership>()))
            .ReturnsAsync((null!, "User is already a member of this clan."));

        // Act
        var result = await _controller.JoinClanWithInvite(input);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorDto>(bad.Value);
        Assert.Equal("User is already a member of this clan.", error.Message);
    }

    [Fact]
    public async Task JoinClanWithInvite_Returns_Ok_With_ReadDto_When_Joined()
    {
        // Arrange
        var userId = "user-1";
        SetUser(userId);
        var input = new InviteCodeDto { InviteCode = "OK" };
        var clanId = Guid.NewGuid();

        var invitation = new ClanInvitation
        {
            InviteId = Guid.NewGuid(),
            ClanId = clanId,
            InviteCode = "OK",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            MaxUses = 10,
            UsedCount = 0,
            IsActive = true
        };

        _clanServiceMock.Setup(s => s.ValidateAndUseInvitationAsync(input.InviteCode))
            .ReturnsAsync((true, "ok", invitation));

        var createdMembership = new ClanMembership
        {
            Id = Guid.NewGuid(),
            ClanId = clanId,
            UserId = userId,
            Role = "Member"
        };

        _membershipServiceMock.Setup(s => s.AddMemberAsync(It.Is<ClanMembership>(m =>
                m.ClanId == clanId &&
                m.UserId == userId
                // Role is set in controller; we avoid strict check to prevent brittle tests
            )))
            .ReturnsAsync((createdMembership, "ok"));

        // Act
        var result = await _controller.JoinClanWithInvite(input);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ClanMembershipReadDto>(ok.Value);
        Assert.Equal(createdMembership.Id, dto.Id);
        Assert.Equal(createdMembership.ClanId, dto.ClanId);
        Assert.Equal(createdMembership.UserId, dto.UserId);
        Assert.Equal(createdMembership.Role, dto.Role);
    }

    #endregion
}