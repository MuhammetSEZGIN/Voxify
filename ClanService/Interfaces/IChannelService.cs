using ClanService.Models;

namespace ClanService.Interfaces
{
    public interface IChannelService
    {
        Task<Channel> CreateChannelAsync(Channel channel);
        Task<Channel> GetChannelByIdAsync(Guid channelId);
        Task<List<Channel>> GetChannelsByClanIdAsync(Guid clanId);
        Task<Channel> UpdateChannelAsync(Channel channel);
        Task<bool> DeleteChannelAsync(Guid channelId);
    }
}
