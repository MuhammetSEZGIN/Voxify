using ClanService.Data;
using ClanService.Interfaces.Repositories;
using ClanService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Repositories;

public class ChannelRepository : Repository<Channel, Guid>, IChannelRepository
{
    public ChannelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Channel>> GetChannelsByClanIdAsync(Guid clanId)
    {
        return await _context.Channels
            .AsNoTracking()
            .Where(x => x.ClanId == clanId)
            .ToListAsync();
    }

    public async Task<bool> DeleteChannelsByClanIdAsync(Guid clanId)
    {
        var channels = await _context.Channels
            .Where(x => x.ClanId == clanId)
            .ToListAsync();

        if (!channels.Any())
            return false;

        _context.Channels.RemoveRange(channels);
        await _context.SaveChangesAsync();
        return true;
    }
}