using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClanService.Enums;

namespace ClanService.Models
{
    /// <summary>
    /// Kullanıcıların hangi clanlara üye olduğunu takip eden tablo.
    /// </summary>
    public class ClanMembership
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Kullanıcının üye olduğu clan bilgisi.
        /// </summary>
        [ForeignKey(nameof(Clan))]
        public Guid ClanId { get; set; }
        public Clan Clan { get; set; }

        /// <summary>
        /// Identity servisinden gelen kullanıcı kimliği.
        /// </summary>
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }

        public string Role { get; set; }
    }
}
