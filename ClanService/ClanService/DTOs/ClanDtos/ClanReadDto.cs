using System.Threading.Channels;

namespace ClanService.DTOs
{
    public class ClanReadDto
    {
        public Guid ClanId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
     
    }
}