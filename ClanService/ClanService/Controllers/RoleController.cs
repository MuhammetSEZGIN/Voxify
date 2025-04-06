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
        private readonly ILogger<RoleController> _logger;
        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPut("{membershipId}")]
        public async Task<IActionResult> UpdateRoleAsync(UpdateRoleDto roleDto)
        {
            var result = await _roleService.UpdateRoleAsync(roleDto.MembershipId, roleDto.RoleName);
            if(!result)
            {
                return BadRequest();
            }
            return Ok();
        }

    }
}
