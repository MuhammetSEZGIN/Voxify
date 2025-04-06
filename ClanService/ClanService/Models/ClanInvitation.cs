using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClanService.Models;

public class ClanInvitation
{
    [Key]
    public Guid InviteId { get; set; }
    public Guid ClanId { get; set; }
    [ForeignKey("ClanId")]
    public Clan Clan { get; set; }
    public bool IsActive { get; set; }
    [Required]

    public DateTime ExpiresAt { get; set; }
    [Required]
    public string InviteCode { get; set; }
    [Required]
    public int MaxUses { get; set; }
    [Required]
    public int UsedCount { get; set; }
}
