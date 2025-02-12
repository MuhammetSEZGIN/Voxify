using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClanService.Models
{
    /// <summary>
    /// Bir ses kanalı modeli, clan içinde gerçek zamanlı ses iletişimini sağlar.
    /// Örneğin WebRTC, SignalR veya başka bir iletişim protokolü aracılığıyla entegre edilebilir.
    /// </summary>
    public class VoiceChannel
    {
        [Key]
        public Guid VoiceChannelId { get; set; }

        /// <summary>
        /// Ses kanalının ismi.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Ses kanalının hangi clan'a ait olduğunu belirtir.
        /// </summary>
        [ForeignKey(nameof(Clan))]
        public Guid ClanId { get; set; }
        public Clan Clan { get; set; }

        /// <summary>
        /// Diğer ihtiyaç duyulan ek bilgiler (ör. Maks katılımcı sayısı, aktif mi vb.).
        /// Burada veya ayrı bir tablo/model üzerinden yönetilebilir.
        /// </summary>
        public bool IsActive { get; set; } = true;
        [Range(1, 10)]
        public int MaxParticipants { get; set; }

        
    }
}
