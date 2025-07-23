using IdentityService.DTOs;
using IdentityService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // POST api/role/create?roleName=Admin
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role name is required.");

            var result = await _roleService.CreateRoleAsync(roleName);
            if (result.Succeeded)
                return Ok($"Role '{roleName}' created successfully.");

            return BadRequest(result.Errors);
        }

        // DELETE api/role/delete?roleName=Admin
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRole([FromQuery] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role name is required.");

            var result = await _roleService.DeleteRoleAsync(roleName);
            if (result.Succeeded)
                return Ok($"Role '{roleName}' deleted successfully.");

            return BadRequest(result.Errors);
        }

        // POST api/role/assign
        // Body example: { "userId": "userid", "roleName": "Admin" }
        [HttpPost("assign")]
        public async Task<IActionResult> AssignUserToRole([FromBody] AssignRoleDto dto)
        {
            if (
                dto == null
                || string.IsNullOrWhiteSpace(dto.UserId)
                || string.IsNullOrWhiteSpace(dto.RoleName)
            )
                return BadRequest("Invalid role assignment data.");

            var result = await _roleService.AssignUserToRoleAsync(dto.UserId, dto.RoleName);
            if (result.Succeeded)
                return Ok($"Assigned role '{dto.RoleName}' to user '{dto.UserId}'.");

            return BadRequest(result.Errors);
        }

        // GET api/role/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required.");

            var roles = await _roleService.GetUserRolesAsync(userId);
            return Ok(roles);
        }

        // GET api/role/isinrole?userId=userid&roleName=Admin
        [HttpGet("isinrole")]
        public async Task<IActionResult> IsUserInRole(
            [FromQuery] string userId,
            [FromQuery] string roleName
        )
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Both user ID and role name are required.");

            var isInRole = await _roleService.IsUserInRoleAsync(userId, roleName);
            return Ok(
                new
                {
                    userId,
                    roleName,
                    isInRole,
                }
            );
        }
    }
}
