using System;
using MessageService.Models;

namespace MessageService.Interfaces;

public interface IMessageService
{
    Task<IEnumerable<Message>> GetMessagesInChannelAsync(Guid channelId, int limit);
    Task<Message> UpdateMessage(Guid messageId, string newContent);
    Task DeleteMessage(Guid messageId);
    Task<Message> CreateMessage(Message message);
}
