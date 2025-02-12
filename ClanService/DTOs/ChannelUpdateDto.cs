using System.ComponentModel.DataAnnotations;

namespace ClanService.DTOs
{
    public class ChannelUpdateDto
    {
        [Required]
        public Guid ChannelId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}