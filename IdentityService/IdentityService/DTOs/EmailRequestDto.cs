using System;

namespace IdentityService.DTOs;

public class EmailRequestDto
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}
