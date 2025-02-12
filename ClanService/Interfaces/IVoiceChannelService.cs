using ClanService.Models;

namespace ClanService.Interfaces
{
    public interface IVoiceChannelService
    {
        Task<VoiceChannel> CreateVoiceChannelAsync(VoiceChannel voiceChannel);
        Task<VoiceChannel> GetVoiceChannelByIdAsync(Guid voiceChannelId);
        Task<List<VoiceChannel>> GetVoiceChannelsByClanIdAsync(Guid clanId);
        Task<VoiceChannel> UpdateVoiceChannelAsync(VoiceChannel voiceChannel);
        Task<bool> DeleteVoiceChannelAsync(Guid voiceChannelId);
    }
}
