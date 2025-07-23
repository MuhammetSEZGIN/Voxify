using System;

namespace IdentityService.DTOs;

public class RefreshTokenDto
{
    public string UserId { get; set; }
    public string RefreshToken { get; set; }
    public string DeviceInfo { get; set; }
}
