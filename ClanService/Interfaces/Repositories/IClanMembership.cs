using System;
using ClanService.Models;

namespace ClanService.Interfaces.Repositories;

public interface IClanMembershipRepository : IRepository<ClanMembership, Guid>
{
    Task<IEnumerable<ClanMembership>> GetMembersByClanIdAsync(Guid clanId);
    Task<ClanMembership> GetMemberByUserAndClanIdAsync(string userId, Guid clanId);
    Task<List<ClanMembership>> GetByUserIdAsync(string userId);
    Task<bool> DeleteMembersByClanIdAsync(Guid clanId);
}