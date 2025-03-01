using System;
using MessageService.Models;

namespace MessageService.Interfaces;

public interface IMessageService
{
    Task<ServiceResult<IEnumerable<Message>>> GetMessagesInChannelAsync(Guid channelId, int limit, int page);
    Task <ServiceResult<Message>>UpdateMessage(Guid messageId, string newContent);
    Task<ServiceResult<bool>> DeleteMessageAsync(Guid messageId);
    Task<ServiceResult<Message>> CreateMessage(Message message);
}
