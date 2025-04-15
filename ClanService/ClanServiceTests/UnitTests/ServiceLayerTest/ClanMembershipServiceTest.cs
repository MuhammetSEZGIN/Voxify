using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ClanMembershipServiceTest
    {
        private Mock<IClanMembershipRepository> _mockMembershipRepository;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILogger<ClanMembershipService>> _mockLogger;
        private ClanMembershipService _membershipService;

        [TestInitialize]
        public void Setup()
        {
            _mockMembershipRepository = new Mock<IClanMembershipRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<ClanMembershipService>>();
            
            _membershipService = new ClanMembershipService(
                _mockMembershipRepository.Object,
                _mockUserRepository.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task AddMemberAsync_WithValidData_ShouldAddMember()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                UserId = userId,
                ClanId = clanId,
                Role = Enums.ClanRole.Member.ToString()
            };

            var user = new User { Id = userId, Username = "TestUser" };

            _mockMembershipRepository.Setup(r => r.GetMemberByUserAndClanIdAsync(userId, clanId))
                .ReturnsAsync((ClanMembership)null);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockMembershipRepository.Setup(r => r.AddAsync(It.IsAny<ClanMembership>()))
                .ReturnsAsync(membership);

            // Act
            var result = await _membershipService.AddMemberAsync(membership);

            // Assert
            Assert.IsNotNull(result.Item1, "Membership should not be null");
            Assert.AreEqual("User added to the clan successfully.", result.Item2);
            Assert.AreNotEqual(Guid.Empty, result.Item1.Id, "Membership ID should be generated");
            Assert.AreEqual(userId, result.Item1.UserId);
            Assert.AreEqual(clanId, result.Item1.ClanId);
            
            _mockMembershipRepository.Verify(r => r.AddAsync(It.IsAny<ClanMembership>()), Times.Once);
        }

        [TestMethod]
        public async Task AddMemberAsync_WhenUserAlreadyMember_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            var existingMembership = new ClanMembership
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClanId = clanId,
                Role = Enums.ClanRole.Member.ToString()
            };
            
            var membership = new ClanMembership
            {
                UserId = userId,
                ClanId = clanId,
                Role = Enums.ClanRole.Member.ToString()
            };

            _mockMembershipRepository.Setup(r => r.GetMemberByUserAndClanIdAsync(userId, clanId))
                .ReturnsAsync(existingMembership);

            // Act
            var result = await _membershipService.AddMemberAsync(membership);

            // Assert
            Assert.IsNull(result.Item1, "Membership should be null");
            Assert.AreEqual("User is already a member of this clan.", result.Item2);
            
            _mockMembershipRepository.Verify(r => r.AddAsync(It.IsAny<ClanMembership>()), Times.Never);
        }

        [TestMethod]
        public async Task AddMemberAsync_WhenUserNotFound_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                UserId = userId,
                ClanId = clanId,
                Role = Enums.ClanRole.Member.ToString()
            };

            _mockMembershipRepository.Setup(r => r.GetMemberByUserAndClanIdAsync(userId, clanId))
                .ReturnsAsync((ClanMembership)null);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _membershipService.AddMemberAsync(membership);

            // Assert
            Assert.IsNull(result.Item1, "Membership should be null");
            Assert.AreEqual("User not found.", result.Item2);
            
            _mockMembershipRepository.Verify(r => r.AddAsync(It.IsAny<ClanMembership>()), Times.Never);
        }

        [TestMethod]
        public async Task AddMemberAsync_WhenExceptionOccurs_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                UserId = userId,
                ClanId = clanId,
                Role = Enums.ClanRole.Member.ToString()
            };

            var user = new User { Id = userId, Username = "TestUser" };

            _mockMembershipRepository.Setup(r => r.GetMemberByUserAndClanIdAsync(userId, clanId))
                .ReturnsAsync((ClanMembership)null);
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockMembershipRepository.Setup(r => r.AddAsync(It.IsAny<ClanMembership>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _membershipService.AddMemberAsync(membership);

            // Assert
            Assert.IsNull(result.Item1, "Membership should be null");
            Assert.AreEqual("An error occurred while adding user to the clan.", result.Item2);
            
            // Verify that exception is logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetMembershipAsync_ShouldReturnMembership()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                Id = membershipId,
                UserId = Guid.NewGuid().ToString(),
                ClanId = Guid.NewGuid(),
                Role = Enums.ClanRole.Member.ToString()
            };

            _mockMembershipRepository.Setup(r => r.GetByIdAsync(membershipId))
                .ReturnsAsync(membership);

            // Act
            var result = await _membershipService.GetMembershipAsync(membershipId);

            // Assert
            Assert.IsNotNull(result, "Membership should not be null");
            Assert.AreEqual(membershipId, result.Id);
        }

        [TestMethod]
        public async Task GetMembershipsByClanIdAsync_ShouldReturnListOfMemberships()
        {
            // Arrange
            var clanId = Guid.NewGuid();
            var memberships = new List<ClanMembership>
            {
                new ClanMembership { Id = Guid.NewGuid(), ClanId = clanId, UserId = Guid.NewGuid().ToString(), Role = Enums.ClanRole.Member.ToString() },
                new ClanMembership { Id = Guid.NewGuid(), ClanId = clanId, UserId = Guid.NewGuid().ToString(), Role = Enums.ClanRole.Admin.ToString() }
            };

            _mockMembershipRepository.Setup(r => r.GetMembersByClanIdAsync(clanId))
                .ReturnsAsync(memberships);

            // Act
            var result = await _membershipService.GetMembershipsByClanIdAsync(clanId);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(2, result.Count, "Should return 2 memberships");
            Assert.IsTrue(result.All(m => m.ClanId == clanId), "All memberships should have the specified clan ID");
        }

        [TestMethod]
        public async Task GetMembershipsByUserIdAsync_ShouldReturnListOfMemberships()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var memberships = new List<ClanMembership>
            {
                new ClanMembership { Id = Guid.NewGuid(), ClanId = Guid.NewGuid(), UserId = userId, Role = Enums.ClanRole.Member.ToString() },
                new ClanMembership { Id = Guid.NewGuid(), ClanId = Guid.NewGuid(), UserId = userId, Role = Enums.ClanRole.Owner.ToString() }
            };

            _mockMembershipRepository.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(memberships);

            // Act
            var result = await _membershipService.GetMembershipsByUserIdAsync(userId);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual(2, result.Count, "Should return 2 memberships");
            Assert.IsTrue(result.All(m => m.UserId == userId), "All memberships should have the specified user ID");
        }

        [TestMethod]
        public async Task LeaveClanAsync_WithValidData_ShouldRemoveMembership()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClanId = clanId,
                Role = Enums.ClanRole.Member.ToString()
            };

            _mockMembershipRepository.Setup(r => r.GetMemberByUserAndClanIdAsync(userId, clanId))
                .ReturnsAsync(membership);
            _mockMembershipRepository.Setup(r => r.DeleteAsync(membership))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _membershipService.LeaveClanAsync(userId, clanId);

            // Assert
            Assert.IsNotNull(result.Item1, "Membership should not be null");
            Assert.AreEqual("User has left the clan successfully.", result.Item2);
            Assert.AreEqual(membership.Id, result.Item1.Id);
            
            _mockMembershipRepository.Verify(r => r.DeleteAsync(membership), Times.Once);
        }

        [TestMethod]
        public async Task LeaveClanAsync_WhenUserNotMember_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();

            _mockMembershipRepository.Setup(r => r.GetMemberByUserAndClanIdAsync(userId, clanId))
                .ReturnsAsync((ClanMembership)null);

            // Act
            var result = await _membershipService.LeaveClanAsync(userId, clanId);

            // Assert
            Assert.IsNull(result.Item1, "Membership should be null");
            Assert.AreEqual("User is not a member of this clan.", result.Item2);
            
            _mockMembershipRepository.Verify(r => r.DeleteAsync(It.IsAny<ClanMembership>()), Times.Never);
        }

        [TestMethod]
        public async Task LeaveClanAsync_WhenExceptionOccurs_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var clanId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClanId = clanId,
                Role = Enums.ClanRole.Member.ToString()
            };

            _mockMembershipRepository.Setup(r => r.GetMemberByUserAndClanIdAsync(userId, clanId))
                .ReturnsAsync(membership);
            _mockMembershipRepository.Setup(r => r.DeleteAsync(membership))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _membershipService.LeaveClanAsync(userId, clanId);

            // Assert
            Assert.IsNull(result.Item1, "Membership should be null");
            Assert.AreEqual("An error occurred while leaving the clan.", result.Item2);
            
            // Verify that exception is logged
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task RemoveMemberAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var membershipId = Guid.NewGuid();
            var membership = new ClanMembership
            {
                Id = membershipId,
                UserId = Guid.NewGuid().ToString(),
                ClanId = Guid.NewGuid(),
                Role = Enums.ClanRole.Member.ToString()
            };

            _mockMembershipRepository.Setup(r => r.GetByIdAsync(membershipId))
                .ReturnsAsync(membership);
            _mockMembershipRepository.Setup(r => r.DeleteAsync(membership))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _membershipService.RemoveMemberAsync(membershipId);

            // Assert
            Assert.IsTrue(result, "Result should be true");
            _mockMembershipRepository.Verify(r => r.DeleteAsync(membership), Times.Once);
        }

        [TestMethod]
        public async Task RemoveMemberAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var membershipId = Guid.NewGuid();

            _mockMembershipRepository.Setup(r => r.GetByIdAsync(membershipId))
                .ReturnsAsync((ClanMembership)null);

            // Act
            var result = await _membershipService.RemoveMemberAsync(membershipId);

            // Assert
            Assert.IsFalse(result, "Result should be false");
            _mockMembershipRepository.Verify(r => r.DeleteAsync(It.IsAny<ClanMembership>()), Times.Never);
        }
    }
}