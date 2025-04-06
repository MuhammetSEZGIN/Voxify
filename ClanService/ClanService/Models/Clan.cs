using System.ComponentModel.DataAnnotations;

namespace ClanService.Models
{
    public class Clan
    {
        [Key]
        public Guid ClanId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public List<Channel> Channels { get; set; } = new List<Channel>();
        public List<VoiceChannel> VoiceChannels {get; set;}= new List<VoiceChannel>();
        public string ImagePath {get; set;}
        public List<ClanMembership> ClanMemberShips { get; set; } = new List<ClanMembership>();
        public List<ClanInvitation> ClanInvitations {get; set;} = new List<ClanInvitation>();
        public string Description { get; set; }
    }

}
