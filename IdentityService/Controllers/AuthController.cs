using System.Net;
using System.Security.Claims;
using IdentityService.Attributes;
using IdentityService.DTOs;
using IdentityService.Examples;
using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IRegisterService _registerService;

        public AuthController(
            IAuthService authService,
            IEmailService emailService,
            IRegisterService registerService
        )
        {
            _authService = authService;
            _emailService = emailService;
            _registerService = registerService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item #1",
        ///        "isComplete": true
        ///     }
        /// </remarks>
        /// <param name="model">The registration model containing user details.</param>
        [HttpPost("register")]
        [ValidateModel]
        [SwaggerRequestExample(typeof(RegisterModel), typeof(RegisterRequestExample))]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _registerService.RegisterAsync(model);

            var confirmationUrl = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email";

            if (!result.IsSuccessfull)
            {
                return new ObjectResult(result) { StatusCode = result.StatusCode };
            }

            if (result.IsSuccessfull)
            {
                var emailResult = await _emailService.SendEmailConfirmationAsync(
                    result.Data.UserId,
                    confirmationUrl
                );
            }

            return new ObjectResult(
                new RegisterResponseDto
                {
                    UserId = result.Data.UserId,
                    RefreshToken = result.Data.RefreshToken,
                    Token = result.Data.Token,
                }
            )
            {
                StatusCode = result.StatusCode,
            };
        }

        [HttpPost("login")]
        [ValidateModel]
        [SwaggerRequestExample(typeof(LoginRequestModel), typeof(LoginRequestExample))]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
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

        [HttpPost("logout-session/{sessionId}")]
        public async Task<IActionResult> LogoutSession(string sessionId)
        {
            var result = await _authService.LogoutSessionAsync(sessionId);
            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }

        [HttpPost("send-email-test")]
        public async Task<IActionResult> SendEmailTest([FromBody] EmailRequestDto model)
        {
            var result = await _emailService.SendEmailAsync(
                model.ToEmail,
                model.Subject,
                model.Content
            );
            return new ObjectResult(result) { StatusCode = result.StatusCode };
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(
            [FromQuery] string userId,
            [FromQuery] string token
        )
        {
            var result = await _emailService.ConfirmEmail(userId, token);
            return new ObjectResult(result.Data) { StatusCode = result.StatusCode };
        }
    }
}
