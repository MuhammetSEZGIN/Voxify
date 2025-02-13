using System.ComponentModel.DataAnnotations;

namespace IdentityService.DTOs
{
    public class UpdateUserModel
    {
        [Required]
        public string Id { get; set; }
                
        [Required]
        public string UserName { get; set; }
                
        [Required]
        public string Email { get; set; }
                
        public string FullName { get; set; }
                
        public string AvatarUrl { get; set; }
    }
}