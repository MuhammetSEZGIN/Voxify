using System;
using IdentityService.DTOs;
using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IRegisterService
{
    Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterModel model);
    Task<bool> IsEmailTakenAsync(string email);
    Task<bool> IsUsernameTakenAsync(string username);
}
