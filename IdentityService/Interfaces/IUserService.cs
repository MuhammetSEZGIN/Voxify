using System;
using IdentityService.DTOs;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Interfaces;

public interface IUserService
{
    Task<IdentityResult> UpdateUserAsync(UpdateUserModel model);

}
