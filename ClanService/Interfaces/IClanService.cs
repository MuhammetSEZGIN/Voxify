using ClanService.Models;

namespace ClanService.Interfaces
{
    public interface IClanService
    {
        Task<Clan> CreateClanAsync(Clan clan);
        Task<Clan> GetClanByIdAsync(Guid clanId);
        Task<List<Clan>> GetAllClansAsync();
        Task<Clan> UpdateClanAsync(Clan clan);
        Task<bool> DeleteClanAsync(Guid clanId);
        Task<List<Clan>> GetClansByUserIdAsync(string userId);

    }
}
