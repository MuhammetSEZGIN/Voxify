using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models;

public class UserRefreshToken
{
    [Key]
    public string Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string RefreshToken { get; set; }

    [Required]
    public DateTime RefreshTokenExpiryTime { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    public string CreatedByIp { get; set; }

    [Required]
    public string DeviceInfo { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; }

    [NotMapped]
    public bool IsActive => RefreshTokenExpiryTime >= DateTime.UtcNow;
}
