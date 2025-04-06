using System;
using ClanService.Models;

namespace ClanService.Interfaces.Repositories;

public interface IClanRepository : IRepository<Clan, Guid>
{
    Task<IEnumerable<Clan>> GetClansByUserIdAsync(string userId);
    Task<IEnumerable<Clan>> SearchClansAsync(string searchText, int limit, int page);
    Task<Clan>GetClanWithDetailsAsync(Guid id);
}