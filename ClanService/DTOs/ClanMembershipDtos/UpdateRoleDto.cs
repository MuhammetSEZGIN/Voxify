using System;

namespace ClanService.DTOs.ClanMembershipDtos;

public class UpdateRoleDto
{
    public string RoleName { get; set; }
    public Guid MembershipId { get; set; }
}
