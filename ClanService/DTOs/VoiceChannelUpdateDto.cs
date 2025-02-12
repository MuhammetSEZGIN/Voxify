using System.ComponentModel.DataAnnotations;

namespace ClanService.DTOs
{
    public class VoiceChannelUpdateDto
    {
        [Required]
        public Guid VoiceChannelId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}