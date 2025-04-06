using ClanService.DTOs;
using ClanService.Models;

namespace ClanService.Interfaces
{
    public interface IClanService
    {
        Task<(Clan, string)> CreateClanAsync(Clan clan, string userId);
        Task<Clan> GetClanByIdAsync(Guid clanId);
        Task<IEnumerable<Clan>> GetAllClansAsync();
        Task<Clan> UpdateClanAsync(Clan clan);
        Task<bool> DeleteClanAsync(Guid clanId);
        Task<List<Clan>> GetClansByUserIdAsync(string userId);
        Task<ClanInvitation> CreateInviteTokenAsync(Guid clanId, TimeSpan? expipreInHours = null, int? maxUses = null);
        Task<(bool, string, ClanInvitation)> ValidateAndUseInvitationAsync(string code);
        Task<ClanInvitation> GetInvitationByCodeAsync(string code);

    }

}
