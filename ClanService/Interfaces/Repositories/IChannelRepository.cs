using System;
using ClanService.Models;

namespace ClanService.Interfaces.Repositories;

public interface IChannelRepository : IRepository<Channel, Guid>
{
    Task<IEnumerable<Channel>> GetChannelsByClanIdAsync(Guid clanId);
    Task<bool> DeleteChannelsByClanIdAsync(Guid clanId);
}