using System.ComponentModel.DataAnnotations;

namespace ClanService.DTOs
{
    public class ClanUpdateDto
    {
        [Required]
        public Guid ClanId { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
    }
}