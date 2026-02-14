using System;

namespace IdentityService.DTOs;

public class RefreshTokenResultDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
