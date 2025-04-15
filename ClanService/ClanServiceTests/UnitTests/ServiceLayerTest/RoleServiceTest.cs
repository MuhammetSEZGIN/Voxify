using System;
using System.Threading.Tasks;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using ClanService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClanService.Services.Tests
{
    [TestClass]
    public class RoleServiceTest
    {
        private Mock<IClanMembershipRepository> _mockMembershipRepository;
        private Mock<ILogger<RoleService>> _mockLogger;
        private RoleService _roleService;

        [TestInitialize]
        public void Setup()
        {
            _mockMembershipRepository = new Mock<IClanMembershipRepository>();
            _mockLogger = new Mock<ILogger<RoleService>>();
            
            _roleService = new RoleService(
                _mockMembershipRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task UpdateRoleAsync_WithValidMembershipAndRole_ShouldUpdateRoleAndReturnTrue()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            var oldRole = "Member";
            var newRole = "Admin";
            
            var membership = new ClanMembership
            {
                Id = membershipId,
                UserId = "testUserId",
                ClanId = Guid.NewGuid(),
                Role = oldRole
            };

            _mockMembershipRepository.Setup(r => r.GetByIdAsync(membershipId))
                .ReturnsAsync(membership);
            _mockMembershipRepository.Setup(r => r.UpdateAsync(It.IsAny<ClanMembership>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _roleService.UpdateRoleAsync(membershipId, newRole);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(newRole, membership.Role);
            
            // Verify repository method was called
            _mockMembershipRepository.Verify(r => r.UpdateAsync(It.Is<ClanMembership>(m => 
                m.Id == membershipId && m.Role == newRole)), Times.Once);
            
            // Verify successful log message was written
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, type) => 
                        obj.ToString().Contains("updated to")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateRoleAsync_WithNonExistentMembership_ShouldReturnFalse()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            var newRole = "Admin";

            _mockMembershipRepository.Setup(r => r.GetByIdAsync(membershipId))
                .ReturnsAsync((ClanMembership)null);

            // Act
            var result = await _roleService.UpdateRoleAsync(membershipId, newRole);

            // Assert
            Assert.IsFalse(result);
            
            // Verify update was never called
            _mockMembershipRepository.Verify(r => r.UpdateAsync(It.IsAny<ClanMembership>()), Times.Never);
            
            // Verify warning log message was written
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((obj, type) => 
                        obj.ToString().Contains("Membership not found")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

    }
}