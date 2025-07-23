using System;
using IdentityService.DTOs;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<RegisterModel>> RegisterAsync(RegisterModel model);
    Task<ApiResponse<string>> LoginAsync(LoginRequestModel model);
    Task<ApiResponse<RefreshTokenResultDto>> RefreshTokenAsync(RefreshTokenDto model);
    Task<ApiResponse<List<UserSessionsResultDto>>> GetMySessionsByUserId(string userId);
    Task<ApiResponse<string>> LogoutSessionAsync(int sessionId);
}
