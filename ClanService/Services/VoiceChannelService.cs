using ClanService.Data;
using ClanService.Models;
using ClanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClanService.Services
{
    public class VoiceChannelService : IVoiceChannelService
    {
        private readonly ApplicationDbContext _context;

        public VoiceChannelService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(VoiceChannel, string)> CreateVoiceChannelAsync(VoiceChannel voiceChannel)
        {
            var clan = await _context.Clans.FindAsync(voiceChannel.ClanId);
            if(clan == null) 
                return (null, "Clan not found");
            
            await _context.VoiceChannels.AddAsync(voiceChannel);
            await _context.SaveChangesAsync();
            return (voiceChannel, "VoiceChannel created successfully");
        }

        public async Task<VoiceChannel> GetVoiceChannelByIdAsync(Guid voiceChannelId)
        {
            return await _context.VoiceChannels
                .FirstOrDefaultAsync(vc => vc.VoiceChannelId == voiceChannelId);
        }

        public async Task<List<VoiceChannel>> GetVoiceChannelsByClanIdAsync(Guid clanId)
        {
            return await _context.VoiceChannels
                .Where(vc => vc.ClanId == clanId)
                .ToListAsync();
        }

        public async Task<VoiceChannel> UpdateVoiceChannelAsync(VoiceChannel voiceChannel)
        {
            _context.VoiceChannels.Update(voiceChannel);
            await _context.SaveChangesAsync();
            return voiceChannel;
        }

        public async Task<bool> DeleteVoiceChannelAsync(Guid voiceChannelId)
        {
            var existing = await _context.VoiceChannels.FindAsync(voiceChannelId);
            if (existing == null) return false;

            _context.VoiceChannels.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
