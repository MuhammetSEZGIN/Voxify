using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using IdentityService.Data;
using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Messaging;
using IdentityService.Models;
using IdentityService.Utilities;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IdentityProducer _messagePublisher;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IdentityDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        IdentityProducer messagePublisher,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _userManager = userManager;
        _config = config;
        _messagePublisher = messagePublisher;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<RegisterModel>> RegisterAsync(RegisterModel model)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return ApiResponse<RegisterModel>.Failed("This email is already taken");
            }
            existingUser = await _userManager.FindByNameAsync(model.UserName);

            if (existingUser != null)
            {
                return ApiResponse<RegisterModel>.Failed("This username is already taken");
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                AvatarUrl = model.AvatarUrl,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _messagePublisher.PublishUserUpdatedMessageAsync(
                    user.UserName,
                    user.AvatarUrl,
                    user.Id
                );
                _logger.LogInformation(
                    "User created. Username: {Username}, Email: {Email}, FullName: {FullName}, AvatarUrl: {AvatarUrl}",
                    model.UserName,
                    model.Email,
                    model.FullName,
                    model.AvatarUrl
                );
                var userIp = GetIpAdressFromHttpContext();
                var refreshTokenResult = await ControlUserRefreshToken(
                    user,
                    userIp,
                    model.DeviceInfo
                );
                if (!refreshTokenResult)
                {
                    _logger.LogWarning(
                        "Failed to generate refresh token for user: {Username}",
                        user.UserName
                    );
                    return ApiResponse<RegisterModel>.Failed(
                        "User created but failed to generate refresh token",
                        null,
                        (int)HttpStatusCode.InternalServerError
                    );
                }
                return ApiResponse<RegisterModel>.Success(
                    model,
                    "User registered successfully and logged in",
                    (int)HttpStatusCode.Created
                );
            }
            _logger.LogWarning("User registration failed: {Errors}", result.Errors);
            return ApiResponse<RegisterModel>.Failed(
                "User registration failed",
                result.Errors.Select(e => e.Description),
                (int)HttpStatusCode.BadRequest
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unexpected error occurred during user registration for: {Email}",
                model.Email
            );
            return ApiResponse<RegisterModel>.Failed(
                "An error occurred during registration",
                new List<string>() { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }

    private string GetIpAdressFromHttpContext()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ControlUserRefreshToken(
        ApplicationUser user,
        string userIp,
        string deviceInfo
    )
    {
        if (
            user.RefreshTokens == null
            || user.RefreshTokens.Count == 0
            || user.RefreshTokens.Any(rt => rt.IsActive == false)
        )
        {
            var newRefreshToken = new UserRefreshToken
            {
                RefreshToken = GenerateToken.GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                CreatedByIp = userIp,
                DeviceInfo = deviceInfo,
            };
            user.RefreshTokens.Add(newRefreshToken);
            var updateResult = await _userManager.UpdateAsync(user);
            return updateResult.Succeeded;
        }
        return true;
    }

    public async Task<ApiResponse<string>> LoginAsync(LoginRequestModel model)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                _logger.LogWarning("User not found with username: {0}", model.UserName);
                return ApiResponse<string>.Failed("There is no user with this username");
            }
            var canSignIn = await _signInManager.CanSignInAsync(user);
            if (!canSignIn)
            {
                return ApiResponse<string>.Failed(
                    "User cannot sign in. Please confirm your email or contact support."
                );
            }
            SignInResult result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded) { }

            _logger.LogInformation("User logged in: {0}", model.UserName);
            string token = GenerateToken.GenerateJSONWebToken(user, _config);
            var refreshTokenResult = await ControlUserRefreshToken(
                user,
                model.DeviceInfo,
                GetIpAdressFromHttpContext()
            );
            if (!refreshTokenResult)
            {
                _logger.LogWarning(
                    "Failed to update refresh token for user: {Username}",
                    user.UserName
                );
                return ApiResponse<string>.Failed(
                    "Login successful but failed to update refresh token",
                    null,
                    (int)HttpStatusCode.InternalServerError
                );
            }
            return ApiResponse<string>.Success(token, "Login successful", (int)HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during login for user: {0}", model.UserName);
            return ApiResponse<string>.Failed(
                "An error occurred during login",
                new List<string> { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ApiResponse<RefreshTokenResultDto>> RefreshTokenAsync(RefreshTokenDto model)
    {
        try
        {
            var refreshToken = _context
                .UserRefreshTokens.Include(rt => rt.User)
                .SingleOrDefault(rt => rt.RefreshToken == model.RefreshToken);
            if (refreshToken == null)
            {
                return ApiResponse<RefreshTokenResultDto>.Failed(
                    "Invalid refresh token",
                    null,
                    (int)HttpStatusCode.Unauthorized
                );
            }
            if (!refreshToken.IsActive)
            {
                _logger.LogWarning(
                    "İptal edilmiş veya süresi dolmuş bir refresh token kullanıldı: {token}",
                    model.RefreshToken
                );

                return ApiResponse<RefreshTokenResultDto>.Failed(
                    "Refresh token is expired",
                    null,
                    (int)HttpStatusCode.Unauthorized
                );
            }

            string newToken = GenerateToken.GenerateJSONWebToken(refreshToken.User, _config);
            refreshToken.RefreshToken = GenerateToken.GenerateRefreshToken();
            refreshToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            refreshToken.DeviceInfo = model.DeviceInfo;
            refreshToken.CreatedByIp = GetIpAdressFromHttpContext();

            _context.UserRefreshTokens.Update(refreshToken);
            var updateResult = await _context.SaveChangesAsync();

            return ApiResponse<RefreshTokenResultDto>.Success(
                new RefreshTokenResultDto
                {
                    AccessToken = newToken,
                    RefreshToken = refreshToken.RefreshToken,
                },
                "Refresh token successful",
                (int)HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred during refresh token for user: {0}",
                model.UserId
            );
            return ApiResponse<RefreshTokenResultDto>.Failed(
                "An error occurred during refresh token",
                new List<string> { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ApiResponse<List<UserSessionsResultDto>>> GetMySessionsByUserId(string userId)
    {
        var sessions = await _context
            .UserRefreshTokens.Where(rt => rt.UserId == userId)
            .Select(rt => new UserSessionsResultDto
            {
                Id = rt.Id,
                DeviceInfo = rt.DeviceInfo,
                CreatedAt = rt.CreatedAt,
                CreatedByIp = rt.CreatedByIp,
            })
            .ToListAsync();

        if (sessions == null || !sessions.Any())
        {
            return ApiResponse<List<UserSessionsResultDto>>.Success(
                sessions,
                "User sessions retrieved successfully",
                (int)HttpStatusCode.OK
            );
        }
        return ApiResponse<List<UserSessionsResultDto>>.Failed(
            "No sessions found for this user",
            null,
            (int)HttpStatusCode.NotFound
        );
    }

    public async Task<ApiResponse<string>> LogoutSessionAsync(int sessionId)
    {
        try
        {
            var session = _context.UserRefreshTokens.Find(sessionId);
            if (session == null)
            {
                return ApiResponse<string>.Failed(
                    "Session not found",
                    null,
                    (int)HttpStatusCode.NotFound
                );
            }
            if (!session.IsActive)
            {
                return ApiResponse<string>.Failed(
                    "Session is already inactive",
                    null,
                    (int)HttpStatusCode.BadRequest
                );
            }
            _context.UserRefreshTokens.Remove(session);
            var result = await _context.SaveChangesAsync();
            return ApiResponse<string>.Success(
                "",
                "Session logged out successfully",
                (int)HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while logging out session with ID: {0}",
                sessionId
            );
            return ApiResponse<string>.Failed(
                "An error occurred while logging out session",
                new List<string> { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }
}
