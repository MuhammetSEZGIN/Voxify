using System;
using IdentityService.DTOs;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Interfaces;

public interface IUserService
{
    Task<IdentityResult> UpdateUserAsync(UpdateUserModel model);
    Task <IdentityResult>  DeleteUserAsync(string id);    
    Task<ApplicationUser> GetUserByIdAsync(string id);
    
}
