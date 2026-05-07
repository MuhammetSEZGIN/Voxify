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

        try
        {
            await _messageRepository.DeleteMessagesOfChannelByChannelId(msg.ChannelId);
            _logger.LogInformation("Kanal komple siliniyor: {ChannelId}", msg.ChannelId);
             
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kanal silme işlemi sırasında hata oluştu: {ChannelId}", msg.ChannelId);
            throw;
        }
    }

    public async Task Consume(ConsumeContext<ClanDeletedMessage> context)
    {
        var msg = context.Message;

        try
        {
            _logger.LogInformation("Klan komple siliniyor: {ClanId}", msg.ClanId);
            await _messageRepository.DeleteMessagesByClanId(msg.ClanId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Klan silme işlemi sırasında hata oluştu: {ClanId}", msg.ClanId);
            throw;
        }
    }
}
