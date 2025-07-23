using System;

namespace IdentityService.Models;

public class LoginRequestModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string DeviceInfo { get; set; }
}
