using System;
using ClanService.Models;

namespace ClanService.Interfaces.Repositories;

public interface IVoiceChannelRepository : IRepository<VoiceChannel, Guid>
{
    Task<IEnumerable<VoiceChannel>> GetVoiceChannelsByClanIdAsync(Guid id);
}
