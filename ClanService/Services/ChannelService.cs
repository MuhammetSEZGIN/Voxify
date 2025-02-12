using ClanService.Data;
using ClanService.Models;
using ClanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Services
{
    public class ChannelService : IChannelService
    {
        private readonly ApplicationDbContext _context;

        public ChannelService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Channel> CreateChannelAsync(Channel channel)
        {
            channel.ChannelId = Guid.NewGuid();
            await _context.Channels.AddAsync(channel);
            await _context.SaveChangesAsync();
            return channel;
        }

        public async Task<Channel> GetChannelByIdAsync(Guid channelId)
        {
            return await _context.Channels
                .FirstOrDefaultAsync(c => c.ChannelId == channelId);
        }

        public async Task<List<Channel>> GetChannelsByClanIdAsync(Guid clanId)
        {
            return await _context.Channels
                .Where(c => c.ClanId == clanId)
                .ToListAsync();
        }

        public async Task<Channel> UpdateChannelAsync(Channel channel)
        {
            _context.Channels.Update(channel);
            await _context.SaveChangesAsync();
            return channel;
        }

        public async Task<bool> DeleteChannelAsync(Guid channelId)
        {
            var existing = await _context.Channels.FindAsync(channelId);
            if (existing == null) return false;

            _context.Channels.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
 
    }
}
