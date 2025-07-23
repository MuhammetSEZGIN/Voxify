using System;

namespace IdentityService.DTOs;

public class UserSessionsResultDto
{
    public string Id { get; set; }
    public string DeviceInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByIp { get; set; }
}
