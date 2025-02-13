using System;
using System.ComponentModel.DataAnnotations;

namespace ClanService.Models
{
    /// <summary>
    /// Identity servisinden veya harici kullanıcı yapısından gelen 
    /// temel User bilgisi (Örnek amaçlı tutulan model).
    /// Not: Genellikle bu bilgiler Identity Service içerisinde yönetilir; 
    /// ama mikroservis ortamında, diğer servislerde minimal user modeli 
    /// tutmak isteyebilirsiniz.
    /// </summary>
    public class User
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public ICollection<ClanMembership> ClanMemberships { get; set; }   
    }
}
