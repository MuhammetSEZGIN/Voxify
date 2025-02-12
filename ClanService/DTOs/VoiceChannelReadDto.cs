namespace ClanService.DTOs
{
    public class VoiceChannelReadDto
    {
        public Guid VoiceChannelId { get; set; }
        public string Name { get; set; }
        public Guid ClanId { get; set; }
        public bool IsActive { get; set; }
    }
}