using System.ComponentModel.DataAnnotations;

namespace ClanService.DTOs
{
    public class ClanCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }
}