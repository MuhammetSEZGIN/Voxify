using System.ComponentModel.DataAnnotations;

namespace ClanService.DTOs
{
    public class ClanCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public string UserId { get; set; }
        public string Description { get; set; } 
    }
}