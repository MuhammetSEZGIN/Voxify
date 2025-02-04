using System;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterModel model);
    Task<string> LoginAsync(LoginModel model);
    string GenerateJSONWebToken(IdentityUser user);
}
