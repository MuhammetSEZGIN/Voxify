using ClanService.Models;

namespace ClanService.Interfaces
{
    public interface IClanMembershipService
    {
        Task<(ClanMembership, string)> AddMemberAsync(ClanMembership membership);
        Task<ClanMembership> GetMembershipAsync(Guid membershipId);
        Task<List<ClanMembership>> GetMembershipsByClanIdAsync(Guid clanId);
        Task<List<ClanMembership>> GetMembershipsByUserIdAsync(string userId);
        Task<bool> RemoveMemberAsync(Guid membershipId);
    }
}
