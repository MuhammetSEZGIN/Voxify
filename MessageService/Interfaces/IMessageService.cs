using System;
using MessageService.Models;

namespace MessageService.Interfaces;

public interface IMessageService
{
    Task<IEnumerable<Message>> GetMessagesInChannelAsync(int channelId, int limit);

}
