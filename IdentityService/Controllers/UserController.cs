using IdentityService.DTOs;
using IdentityService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
                
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
                
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            var result = await _userService.UpdateUserAsync(model);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User updated successfully" });
            }
            return BadRequest(result.Errors);
        }
    }
}