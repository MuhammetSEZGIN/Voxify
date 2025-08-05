using System.Net;
using IdentityService.Data;
using IdentityService.DTOs;
using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IdentityDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(
        IdentityDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<RefreshTokenService> logger
    )
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public Task<string> GenerateRefreshTokenAsync()
    {
        // Güvenli refresh token oluştur
        var randomBytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Task.FromResult(Convert.ToBase64String(randomBytes));
    }

    public async Task<ApiResponse<RefreshTokenResultDto>> CreateUserRefreshTokenAsync(
        string userId,
        string deviceInfo,
        string ipAddress
    )
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return ApiResponse<RefreshTokenResultDto>.Failed(
                    "Acccess token creation failed - User does not exist",
                    new List<string> { "User does not exist" },
                    (int)HttpStatusCode.NotFound
                );
            }
            await CleanupExpiredUserTokensAsync(user.Id);
            // Aktif token sayısını kontrol et (max 5 cihaz)
            var activeTokenCount = await _context.UserRefreshTokens.CountAsync(rt =>
                rt.UserId == userId && rt.RefreshTokenExpiryTime > DateTime.UtcNow
            );

            if (activeTokenCount >= 5)
            {
                // En eski tokeni sil
                var oldestToken = await _context
                    .UserRefreshTokens.Where(rt =>
                        rt.UserId == userId && rt.RefreshTokenExpiryTime < DateTime.UtcNow
                        || rt.DeviceInfo == deviceInfo
                    )
                    .FirstAsync();

                _context.UserRefreshTokens.Remove(oldestToken);
            }

            // Yeni token oluştur
            var refreshToken = new UserRefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                RefreshToken = await GenerateRefreshTokenAsync(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                DeviceInfo = deviceInfo,
                UserId = user.Id,
            };

            _context.UserRefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return ApiResponse<RefreshTokenResultDto>.Success(
                new RefreshTokenResultDto
                {
                    AccessToken = refreshToken.RefreshToken,
                    RefreshToken = refreshToken.RefreshToken,
                },
                "Refresh token created successfully."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating refresh token for user: {UserId}", userId);
            return ApiResponse<RefreshTokenResultDto>.Failed(
                "An error occurred while creating refresh token",
                new List<string> { ex.Message },
                (int)HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<UserRefreshToken> GetValidRefreshTokenAsync(string refreshToken)
    {
        return await _context
            .UserRefreshTokens.Include(rt => rt.User)
            .FirstOrDefaultAsync(rt =>
                rt.RefreshToken == refreshToken && rt.RefreshTokenExpiryTime > DateTime.UtcNow
            );
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var token = await _context.UserRefreshTokens.FirstOrDefaultAsync(rt =>
                rt.RefreshToken == refreshToken
            );

            if (token != null)
            {
                _context.UserRefreshTokens.Remove(token);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            return false;
        }
    }

    public async Task<bool> RevokeAllUserTokensAsync(string userId)
    {
        try
        {
            var userTokens = await _context
                .UserRefreshTokens.Where(rt => rt.UserId == userId)
                .ToListAsync();

            _context.UserRefreshTokens.RemoveRange(userTokens);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all tokens for user: {UserId}", userId);
            return false;
        }
    }

    public async Task CleanupExpiredTokensAsync()
    {
        try
        {
            var expiredTokens = await _context
                .UserRefreshTokens.Where(rt => rt.RefreshTokenExpiryTime < DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _context.UserRefreshTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Cleaned up {Count} expired refresh tokens",
                    expiredTokens.Count
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cleanup of expired tokens");
        }
    }

    private async Task CleanupExpiredUserTokensAsync(string userId)
    {
        var expiredTokens = await _context
            .UserRefreshTokens.Where(rt =>
                rt.UserId == userId && rt.RefreshTokenExpiryTime < DateTime.UtcNow
            )
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.UserRefreshTokens.RemoveRange(expiredTokens);
        }
    }
}
