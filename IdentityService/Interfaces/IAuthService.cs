using System;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using IdentityService.DTOs;
namespace IdentityService.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterModel model);
    Task<LoginResult> LoginAsync(LoginModel model);
    string GenerateJSONWebToken(ApplicationUser user);
}
