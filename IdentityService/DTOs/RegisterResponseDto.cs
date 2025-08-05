using System;

namespace IdentityService.DTOs;

public class RegisterResponseDto
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
