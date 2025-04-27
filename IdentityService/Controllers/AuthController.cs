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
            if(!ModelState.IsValid){
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new {Message="Invalid data", Errors=errors});
            }
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
            if(!ModelState.IsValid){
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new {Message="Invalid data", Errors=errors});
            }   
            var loginResult = await _authService.LoginAsync(model);
            if(!loginResult.Succeeded){
                return BadRequest(loginResult.ErrorMessage);
            }
            return Ok(new {Token=loginResult.Token});

        }
     
    }
}
