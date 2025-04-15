using System;

namespace ClanService.DTOs.ClanDtos;

public class InviteClanDto
{
    public string InviteCode { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int MaxUses { get; set; } = 0;
}
