using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;           
        public AuthController(IAuthService authService )
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
           var result= await _authService.RegisterAsync(model);

            if (result.Succeeded)
            {
                // we will detirmine return models later
                return Ok(
                    new{
                        UserName=model.UserName,
                        Email=model.Email,
                        FullName=model.FullName,
                        AvatarUrl=model.AvatarUrl
                    }
                );
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _authService.LoginAsync(model);
            if(String.IsNullOrEmpty(token)){
                return BadRequest("Invalid login attempt");
            }
            return Ok(token);

        }
     
    }
}
