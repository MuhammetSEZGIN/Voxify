using System.ComponentModel.DataAnnotations;

namespace ClanService.DTOs
{
    public class ClanMembershipCreateDto
    {
        [Required]
        public Guid ClanId { get; set; }
        [Required]
        public string UserId { get; set; }

        public string ClanRole { get; set; }  
    }
}