using System.ComponentModel.DataAnnotations;

namespace ClanService.Models
{
    /// <summary>
    /// Bir klan topluluğunu ifade eder. 
    /// Her klanın birden fazla kanalı olabilir.
    /// </summary>
    public class Clan
    {
        [Key]
        public Guid ClanId { get; set; }

        /// <summary>
        /// Klan ismi.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Klanın sahip olduğu kanalların listesi.
        /// </summary>
        public List<Channel> Channels { get; set; } = new List<Channel>();
        public List<VoiceChannel> VoiceChannels {get; set;}= new List<VoiceChannel>();
        public string ImagePath {get; set;}
        /// <summary>
        /// Klan üyelikleri bilgisi (Opsiyonel olarak ClanMemberShip üzerinden ilişki kurulabilir).
        /// ClanMemberShip tablosuyla bire-çok ilişki de tanımlanabilir.
        /// </summary>
        public List<ClanMembership> ClanMemberShips { get; set; } = new List<ClanMembership>();
        public List<ClanInvitation> ClanInvitations {get; set;} = new List<ClanInvitation>();
    }

}
