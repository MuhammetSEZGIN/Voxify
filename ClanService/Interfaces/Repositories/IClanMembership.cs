using System;
using ClanService.Models;

namespace ClanService.Interfaces.Repositories;

public interface IClanMemberShip : IRepository<ClanMembership, Guid>
{
    Task<IEnumerable<ClanMembership>> GetMembersByClanIdAsync(Guid clanId);
    Task<ClanMembership> GetMemberByUserAndClanIdAsync(string userId, Guid clanId);
    Task<bool> DeleteMembersByClanIdAsync(Guid clanId);
}