using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Models;

public class ApplicationUser : IdentityUser
{   
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }

}
