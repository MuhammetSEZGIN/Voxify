using System;
using Shared.Contracts;
using MassTransit;
using MessageService.Interfaces.Repositories.IUserRepository;
using MessageService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MessageService.RabbitMq;

public class ClanServiceConsumer : IConsumer<ChannelDeletedMessage>, IConsumer<ClanDeletedMessage>
{
    IMessageRepository _messageRepository;
    IHubContext<MessageHub> _hubContext;
    ILogger<ClanServiceConsumer> _logger;
    public ClanServiceConsumer(IMessageRepository messageRepository,
                                IHubContext<MessageHub> hubContext,
                                ILogger<ClanServiceConsumer> logger)
    {
        _messageRepository = messageRepository;
        _hubContext = hubContext;
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<ChannelDeletedMessage> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Kanal silme mesajı işleniyor: {ChannelId}", msg.ChannelId);

        await _messageRepository.DeleteMessagesOfChannelByChannelId(msg.ChannelId);
        
        // Presence service de yapmayı planlıyorum
        // await _hubContext.Clients.Group(msg.ChannelId).SendAsync("OnChannelDeleted", msg.ChannelId);
    }

    public async Task Consume(ConsumeContext<ClanDeletedMessage> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Klan komple siliniyor: {ClanId}", msg.ClanId);

        await _messageRepository.DeleteMessagesByClanId(msg.ClanId);

        // Presence service de yapmayı planlıyorum
        // await _hubContext.Clients.Group($"clan_{msg.ClanId}").SendAsync("OnClanDeleted", msg.ClanId);
    }
}
