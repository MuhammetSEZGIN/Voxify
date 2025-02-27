using System;

namespace ClanService.DTOs;

public class UserMembershipDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }

    public string AvatarUrl { get; set; }   
    public string Username { get; set; }
    public string Role { get; set; }
}
