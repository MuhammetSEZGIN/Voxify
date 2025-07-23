using System.Net;
using System.Security.Claims;
using IdentityService.DTOs;
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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Invalid data", Errors = errors });
            }
            var result = await _authService.RegisterAsync(model);

            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return new ObjectResult(ApiResponse<string>.Failed("Invalid data", errors.ToList()))
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                };
            }
            var loginResult = await _authService.LoginAsync(model);
            return new ObjectResult(loginResult) { StatusCode = loginResult.StatusCode };
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            if (string.IsNullOrEmpty(model.RefreshToken) || string.IsNullOrEmpty(model.UserId))
            {
                return new ObjectResult(
                    ApiResponse<string>.Failed("Refresh token or User ID cannot be empty")
                )
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                };
            }

            var result = await _authService.RefreshTokenAsync(model);
            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }

        [HttpGet("my-sessions")]
        public async Task<IActionResult> GetMySessions()
        {
            var result = await _authService.GetMySessionsByUserId(ClaimTypes.NameIdentifier);

            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }

        [HttpPost("logout-session/{sessionId:int}")]
        public async Task<IActionResult> LogoutSession(int sessionId)
        {
            var result = await _authService.LogoutSessionAsync(sessionId);
            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }
    }
}
