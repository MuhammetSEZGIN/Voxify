using System.ComponentModel.DataAnnotations;

namespace ClanService.DTOs
{
    public class ChannelCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid ClanId { get; set; }
    }
}