using System;

namespace IdentityService.DTOs;

public class AuthResponseDto
{
    public string UserID { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
