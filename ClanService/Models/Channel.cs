using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClanService.Models
{
    /// <summary>
    /// Mesajların gönderildiği kanal bilgilerini tutar.
    /// Bir Clan içinde birden fazla kanal olabilir.
    /// </summary>
    public class Channel
    {
        [Key]
        public Guid ChannelId { get; set; }

        /// <summary>
        /// Kanal adı (örn. "Genel", "Oyun Sohbeti" vb.).
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Hangi clan'a ait olduğunu belirlemek için.
        /// </summary>
        [ForeignKey(nameof(Clan))]
        public Guid ClanId { get; set; }

        public Clan Clan { get; set; }

    }
}
