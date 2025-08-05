using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class ApplicationUser : IdentityUser
{
    public string AvatarUrl { get; set; }
    public virtual ICollection<UserRefreshToken> RefreshTokens { get; set; }
}
