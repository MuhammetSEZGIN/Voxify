using System;
using System.Threading.Tasks;
using ClanService.Controllers;
using ClanService.DTOs.ClanMembershipDtos;
using ClanService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClanService.Controllers.Tests
{
    [TestClass]
    public class RoleControllerTest
    {
        private Mock<IRoleService> _mockRoleService;
        private RoleController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockRoleService = new Mock<IRoleService>();
            _controller = new RoleController(_mockRoleService.Object);
        }

        [TestMethod]
        public async Task UpdateRoleAsync_Success_ReturnsOkResult()
        {
            // Arrange
            var roleDto = new UpdateRoleDto
            {
                MembershipId = Guid.NewGuid(),
                RoleName = "Admin"
            };

            _mockRoleService.Setup(s => s.UpdateRoleAsync(roleDto.MembershipId, roleDto.RoleName))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateRoleAsync(roleDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateRoleAsync_Failure_ReturnsBadRequest()
        {
            // Arrange
            var roleDto = new UpdateRoleDto
            {
                MembershipId = Guid.NewGuid(),
                RoleName = "InvalidRole"
            };

            _mockRoleService.Setup(s => s.UpdateRoleAsync(roleDto.MembershipId, roleDto.RoleName))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateRoleAsync(roleDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateRoleAsync_NullRoleDto_ReturnsBadRequest()
        {
            // Arrange & Act
            var result = await _controller.UpdateRoleAsync(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateRoleAsync_EmptyGuid_ReturnsBadRequest()
        {
            // Arrange
            var roleDto = new UpdateRoleDto
            {
                MembershipId = Guid.Empty,
                RoleName = "Admin"
            };

            // Act
            var result = await _controller.UpdateRoleAsync(roleDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateRoleAsync_EmptyRoleName_ReturnsBadRequest()
        {
            // Arrange
            var roleDto = new UpdateRoleDto
            {
                MembershipId = Guid.NewGuid(),
                RoleName = ""
            };

            // Act
            var result = await _controller.UpdateRoleAsync(roleDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }
    }
}