namespace ClanService.DTOs
{
    public class ChannelReadDto
    {
        public Guid ChannelId { get; set; }
        public string Name { get; set; }
        public Guid ClanId { get; set; }
    }
}