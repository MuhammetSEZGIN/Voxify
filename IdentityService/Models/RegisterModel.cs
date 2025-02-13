using System;

namespace IdentityService.Models;

public class RegisterModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
}
