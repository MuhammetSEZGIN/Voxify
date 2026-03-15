using ClanService.DTOs.ClanMembershipDtos;
using ClanService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
      
        }

        [HttpPut("{membershipId}")]
        public async Task<IActionResult> UpdateRoleAsync([FromBody] UpdateRoleDto roleDto)
        {
            if(roleDto == null || roleDto.MembershipId == Guid.Empty || string.IsNullOrEmpty(roleDto.RoleName))
            {
                return BadRequest("Invalid input data.");
            }
            var result = await _roleService.UpdateRoleAsync(roleDto.MembershipId, roleDto.RoleName);
            if(!result)
            {
                return BadRequest("Failed to update role.");
            }
            return Ok();
        }

    }
}
