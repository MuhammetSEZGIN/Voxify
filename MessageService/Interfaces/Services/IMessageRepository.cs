using System;
using MessageService.Models;

namespace MessageService.Interfaces.Services;

public interface IMessageRepository : IRepository<Message,Guid>
{
    Task<IEnumerable<Message>> GetMessagesInChannelAsync(Guid channelId, int limit, int page);
    Task<IEnumerable<Message>> SearchInChannelAsync(Guid channelId, string searchText, int limit, int page);
    Task<bool> DeleteMessagesOfChannelByChannelId(Guid channelId);

}
