namespace ClanService.DTOs
{
    public class ClanMembershipReadDto
    {
        public Guid Id { get; set; }
        public Guid ClanId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}