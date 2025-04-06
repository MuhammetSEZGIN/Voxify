using Microsoft.EntityFrameworkCore;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using ClanService.Data;

namespace ClanService.Repositories;

public class VoiceChannelRepository : Repository<VoiceChannel, Guid>, IVoiceChannelRepository
{
    public VoiceChannelRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<IEnumerable<VoiceChannel>> GetVoiceChannelsByClanIdAsync(Guid id)
    {
        return await _context.VoiceChannels
        .Where(x=>x.ClanId== id)
        .ToListAsync();    
    }
}
