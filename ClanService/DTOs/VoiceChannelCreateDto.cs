using System.ComponentModel.DataAnnotations;

namespace ClanService.DTOs
{
    public class VoiceChannelCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid ClanId { get; set; }
    }
}