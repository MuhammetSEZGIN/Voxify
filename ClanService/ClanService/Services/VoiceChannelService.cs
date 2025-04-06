using ClanService.Models;
using ClanService.Interfaces;
using ClanService.Interfaces.Repositories;

namespace ClanService.Services
{
    public class VoiceChannelService : IVoiceChannelService
    {
        private readonly IClanRepository _clanRepository;
        private readonly IVoiceChannelRepository _voiceChannelRepository;

        public VoiceChannelService(IClanRepository clanRepository, IVoiceChannelRepository voiceChannelRepository)
        {
            _clanRepository = clanRepository;
            _voiceChannelRepository = voiceChannelRepository;
        }

        public async Task<(VoiceChannel, string)> CreateVoiceChannelAsync(VoiceChannel voiceChannel)
        {
            var clan = await _clanRepository.GetByIdAsync(voiceChannel.ClanId);
            if(clan == null) 
                return (null, "Clan not found");
            
            await _voiceChannelRepository.AddAsync(voiceChannel);
            return (voiceChannel, "VoiceChannel created successfully");
        }

        public async Task<VoiceChannel> GetVoiceChannelByIdAsync(Guid voiceChannelId)
        {
            return await _voiceChannelRepository.GetByIdAsync(voiceChannelId);
        }

        public async Task<List<VoiceChannel>> GetVoiceChannelsByClanIdAsync(Guid clanId)
        {
            var channels = await _voiceChannelRepository.GetVoiceChannelsByClanIdAsync(clanId);
            return channels.ToList();
        }

        public async Task<VoiceChannel> UpdateVoiceChannelAsync(VoiceChannel voiceChannel)
        {
            await _voiceChannelRepository.UpdateAsync(voiceChannel);
            return voiceChannel;
        }

        public async Task<bool> DeleteVoiceChannelAsync(Guid voiceChannelId)
        {
            var existing = await _voiceChannelRepository.GetByIdAsync(voiceChannelId);
            if (existing == null) return false;

            await _voiceChannelRepository.DeleteAsync(existing);
            return true;
        }
    }
}
