using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ClanService.Controllers;
using ClanService.DTOs;
using ClanService.DTOs.ClanMembershipDtos;
using ClanService.Enums;
using ClanService.Interfaces;
using ClanService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClanService.Controllers.Tests
{
    [TestClass]
    public class ClanMembershipControllerTest
    {
        private Mock<IClanMembershipService> _mockClanMembershipService;
        private Mock<IClanService> _mockClanService;
        private Mock<IMapper> _mockMapper;
        private ClanMembershipController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockClanMembershipService = new Mock<IClanMembershipService>();
            _mockClanService = new Mock<IClanService>();
            _mockMapper = new Mock<IMapper>();
            
            _controller = new ClanMembershipController(
                _mockClanMembershipService.Object,
                _mockMapper.Object,
                _mockClanService.Object);
        }

        [TestMethod]
        public async Task GetMembership_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                Id = membershipId,
                ClanId = Guid.NewGuid(),
                UserId = Guid.NewGuid().ToString(),
                Role = ClanRole.Member.ToString()
            };
            
            var membershipReadDto = new ClanMembershipReadDto
            {
                Id = membership.Id,
                ClanId = membership.ClanId,
                UserId = membership.UserId,
                Role = membership.Role
            };

            _mockClanMembershipService.Setup(s => s.GetMembershipAsync(membershipId)).ReturnsAsync(membership);
            _mockMapper.Setup(m => m.Map<ClanMembershipReadDto>(It.IsAny<ClanMembership>())).Returns(membershipReadDto);

            // Act
            var result = await _controller.GetMembership(membershipId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(membershipReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task GetMembership_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            _mockClanMembershipService.Setup(s => s.GetMembershipAsync(membershipId)).ReturnsAsync((ClanMembership)null);

            // Act
            var result = await _controller.GetMembership(membershipId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            
            // Verify error message
            var errorDto = notFoundResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("Membership not found.", errorDto.Message);
        }

        [TestMethod]
        public async Task GetMembershipsByClanId_ReturnsOkResult()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var memberships = new List<ClanMembership>
            {
                new ClanMembership { Id = Guid.NewGuid(), ClanId = clanId, UserId = Guid.NewGuid().ToString(), Role = ClanRole.Member.ToString() },
                new ClanMembership { Id = Guid.NewGuid(), ClanId = clanId, UserId = Guid.NewGuid().ToString(), Role = ClanRole.Admin.ToString() }
            };

            var membershipDtos = new List<ClanMembershipReadDto>
            {
                new ClanMembershipReadDto { Id = memberships[0].Id, ClanId = memberships[0].ClanId, UserId = memberships[0].UserId, Role = memberships[0].Role },
                new ClanMembershipReadDto { Id = memberships[1].Id, ClanId = memberships[1].ClanId, UserId = memberships[1].UserId, Role = memberships[1].Role }
            };

            _mockClanMembershipService.Setup(s => s.GetMembershipsByClanIdAsync(clanId)).ReturnsAsync(memberships);
            _mockMapper.Setup(m => m.Map<List<ClanMembershipReadDto>>(It.IsAny<List<ClanMembership>>())).Returns(membershipDtos);

            // Act
            var result = await _controller.GetMembershipsByClanId(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(membershipDtos, okResult.Value);
        }

        [TestMethod]
        public async Task GetMembershipsByUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var memberships = new List<ClanMembership>
            {
                new ClanMembership { Id = Guid.NewGuid(), ClanId = Guid.NewGuid(), UserId = userId, Role = ClanRole.Member.ToString() },
                new ClanMembership { Id = Guid.NewGuid(), ClanId = Guid.NewGuid(), UserId = userId, Role = ClanRole.Owner.ToString() }
            };

            var membershipDtos = new List<ClanMembershipReadDto>
            {
                new ClanMembershipReadDto { Id = memberships[0].Id, ClanId = memberships[0].ClanId, UserId = memberships[0].UserId, Role = memberships[0].Role },
                new ClanMembershipReadDto { Id = memberships[1].Id, ClanId = memberships[1].ClanId, UserId = memberships[1].UserId, Role = memberships[1].Role }
            };

            _mockClanMembershipService.Setup(s => s.GetMembershipsByUserIdAsync(userId)).ReturnsAsync(memberships);
            _mockMapper.Setup(m => m.Map<List<ClanMembershipReadDto>>(It.IsAny<List<ClanMembership>>())).Returns(membershipDtos);

            // Act
            var result = await _controller.GetMembershipsByUserId(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(membershipDtos, okResult.Value);
        }

        [TestMethod]
        public async Task RemoveMember_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            _mockClanMembershipService.Setup(s => s.RemoveMemberAsync(membershipId)).ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveMember(membershipId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task RemoveMember_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            _mockClanMembershipService.Setup(s => s.RemoveMemberAsync(membershipId)).ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveMember(membershipId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            
            // Verify error message
            var errorDto = notFoundResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("Membership not found or already removed.", errorDto.Message);
        }

        [TestMethod]
        public async Task LeaveClan_Success_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                Id = Guid.NewGuid(),
                ClanId = clanId,
                UserId = userId,
                Role = ClanRole.Member.ToString()
            };

            _mockClanMembershipService.Setup(s => s.LeaveClanAsync(userId, clanId))
                .ReturnsAsync((membership, "User has left the clan successfully."));

            // Act
            var result = await _controller.LeaveClan(userId, clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
        }

        [TestMethod]
        public async Task LeaveClan_Failure_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            
            _mockClanMembershipService.Setup(s => s.LeaveClanAsync(userId, clanId))
                .ReturnsAsync((null, "User is not a member of this clan."));

            // Act
            var result = await _controller.LeaveClan(userId, clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
            
            // Verify error message
            var errorDto = notFoundResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("User is not a member of this clan.", errorDto.Message);
        }

        [TestMethod]
        public async Task CreateInvitation_ExistingClanId_ReturnsOkResult()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var clan = new Clan
            {
                ClanId = clanId,
                Name = "Test Clan",
                Description = "Test Description"
            };
            
            var invitation = new ClanInvitation
            {
                InviteId = Guid.NewGuid(),
                ClanId = clanId,
                InviteCode = "ABC123",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                MaxUses = 10,
                UsedCount = 0,
                IsActive = true
            };

            _mockClanService.Setup(s => s.GetClanByIdAsync(clanId)).ReturnsAsync(clan);
            _mockClanService.Setup(s => s.CreateInviteTokenAsync(clanId, null, null)).ReturnsAsync(invitation);

            // Act
            var result = await _controller.CreateInvitation(clanId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            
            // Verify response content
            dynamic responseObject = okResult.Value;
            Assert.AreEqual(invitation.InviteCode, responseObject.InviteCode);
            Assert.AreEqual(invitation.ExpiresAt, responseObject.ExpiresAt);
            Assert.AreEqual(invitation.MaxUses, responseObject.MaxUses);
        }

        [TestMethod]
        public async Task CreateInvitation_NonExistingClanId_ReturnsNotFound()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            _mockClanService.Setup(s => s.GetClanByIdAsync(clanId)).ReturnsAsync((Clan)null);

            // Act
            var result = await _controller.CreateInvitation(clanId);

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
        public async Task JoinClanWithInvite_ValidInvite_ReturnsOkResult()
        {
            // Arrange
            var inviteCode = "ABC123";
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            
            var inviteCodeDto = new InviteCodeDto
            {
                InviteCode = inviteCode,
                UserId = userId
            };
            
            var invitation = new ClanInvitation
            {
                InviteId = Guid.NewGuid(),
                ClanId = clanId,
                InviteCode = inviteCode,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                MaxUses = 10,
                UsedCount = 5
            };
            
            var membership = new ClanMembership
            {
                Id = Guid.NewGuid(),
                ClanId = clanId,
                UserId = userId,
                Role = ClanRole.Member.ToString()
            };
            
            var membershipReadDto = new ClanMembershipReadDto
            {
                Id = membership.Id,
                ClanId = membership.ClanId,
                UserId = membership.UserId,
                Role = membership.Role
            };

            _mockClanService.Setup(s => s.ValidateAndUseInvitationAsync(inviteCode))
                .ReturnsAsync((true, "Invitation code is valid", invitation));
            
            _mockClanMembershipService.Setup(s => s.AddMemberAsync(It.IsAny<ClanMembership>()))
                .ReturnsAsync((membership, "User added to the clan successfully."));
            
            _mockMapper.Setup(m => m.Map<ClanMembershipReadDto>(It.IsAny<ClanMembership>()))
                .Returns(membershipReadDto);

            // Act
            var result = await _controller.JoinClanWithInvite(inviteCodeDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(membershipReadDto, okResult.Value);
        }

        [TestMethod]
        public async Task JoinClanWithInvite_InvalidInvite_ReturnsBadRequest()
        {
            // Arrange
            var inviteCode = "INVALID";
            var userId = Guid.NewGuid().ToString();
            
            var inviteCodeDto = new InviteCodeDto
            {
                InviteCode = inviteCode,
                UserId = userId
            };

            _mockClanService.Setup(s => s.ValidateAndUseInvitationAsync(inviteCode))
                .ReturnsAsync((false, "The code is invalid or inactive", null));

            // Act
            var result = await _controller.JoinClanWithInvite(inviteCodeDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            
            // Verify error message
            var errorDto = badRequestResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("The code is invalid or inactive", errorDto.Message);
        }

        [TestMethod]
        public async Task JoinClanWithInvite_MembershipFailed_ReturnsBadRequest()
        {
            // Arrange
            var inviteCode = "ABC123";
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            
            var inviteCodeDto = new InviteCodeDto
            {
                InviteCode = inviteCode,
                UserId = userId
            };
            
            var invitation = new ClanInvitation
            {
                InviteId = Guid.NewGuid(),
                ClanId = clanId,
                InviteCode = inviteCode,
                IsActive = true,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                MaxUses = 10,
                UsedCount = 5
            };

            _mockClanService.Setup(s => s.ValidateAndUseInvitationAsync(inviteCode))
                .ReturnsAsync((true, "Invitation code is valid", invitation));
            
            _mockClanMembershipService.Setup(s => s.AddMemberAsync(It.IsAny<ClanMembership>()))
                .ReturnsAsync((null, "User is already a member of this clan."));

            // Act
            var result = await _controller.JoinClanWithInvite(inviteCodeDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            
            // Verify error message
            var errorDto = badRequestResult.Value as ErrorDto;
            Assert.IsNotNull(errorDto);
            Assert.AreEqual("User is already a member of this clan.", errorDto.Message);
        }
    }
}