using System;
using System.Net;
using System.Text.Json.Nodes;
using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Messaging;
using IdentityService.Models;
using IdentityService.Utilities;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class RegisterService : IRegisterService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterService> _logger;
    private readonly IIdentityProducer _messagePublisher;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IIpAddressService _ipAddressService;
    private readonly IConfiguration _config;

    public RegisterService(
        UserManager<ApplicationUser> userManager,
        ILogger<RegisterService> logger,
        IIpAddressService ipAddressService,
        IIdentityProducer messagePublisher,
        IRefreshTokenService refreshTokenService,
        IConfiguration config
    )
    {
        _userManager = userManager;
        _logger = logger;
        _ipAddressService = ipAddressService;
        _messagePublisher = messagePublisher;
        _refreshTokenService = refreshTokenService;
        _config = config;
        _messagePublisher = messagePublisher;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterModel model)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                AvatarUrl = model.AvatarUrl,
                EmailConfirmed = false,
                RefreshTokens = new List<UserRefreshToken>(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("User registration failed: {Errors}", result.Errors);
                return ApiResponse<RegisterResponseDto>.Failed(
                    string.Join("; ", result.Errors.Select(e => e.Description))
                );
            }
            await _messagePublisher.PublishUserUpdatedMessageAsync(
                user.UserName,
                user.AvatarUrl,
                user.Id
            );

            _logger.LogInformation(
                "User created. Username: {Username}, Email: {Email}",
                model.UserName,
                model.Email
            );
            var refreshToken = await _refreshTokenService.CreateUserRefreshTokenAsync(
                user.Id,
                model.DeviceInfo,
                _ipAddressService.GetClientIpAddress()
            );

            return ApiResponse<RegisterResponseDto>.Success(
                new RegisterResponseDto
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken.Data.RefreshToken,
                    Token = refreshToken.Data.AccessToken,
                },
                "User registered successfully and logged in",
                (int)HttpStatusCode.Created
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering user: {Email}", model.Email);
            return ApiResponse<RegisterResponseDto>.Failed(
                "An error occurred while registering user",
                new[] { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<bool> IsUsernameTakenAsync(string username)
    {
        var existingUser = await _userManager.FindByNameAsync(username);
        return existingUser != null;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        return existingUser != null;
    }
}
