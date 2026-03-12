using System;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Shared.Contracts;
using PresenceService.Hubs;
using PresenceService.Interfaces;

namespace PresenceService.RabbitMQ;

public class ClanServiceMessageConsumer : IConsumer<ChannelDeletedMessage>
{
    ILogger<ClanServiceMessageConsumer> _logger;
    IPresenceRepository _presenceRepository;
    IHubContext<PresenceHub> _hubContext;

    public ClanServiceMessageConsumer(ILogger<ClanServiceMessageConsumer> logger,
     IPresenceRepository presenceRepository, IHubContext<PresenceHub> hubContext)
    {
        _logger = logger;
        _presenceRepository = presenceRepository;
        _hubContext = hubContext;
    }

  public async Task Consume(ConsumeContext<ChannelDeletedMessage> context)
{
    var message = context.Message;
    
    _logger.LogInformation("Kanal silme mesajı alındı: {ChannelId} (Klan: {ClanId}, Tip: {Type})", 
        message.ChannelId, message.ClanId, message.ChannelType);

    try
    {
        if (string.IsNullOrEmpty(message.ChannelId) || string.IsNullOrEmpty(message.ClanId))
        {
            _logger.LogWarning("Eksik veri içeren kanal silme mesajı alındı.");
            return;
        }

        if (message.ChannelType == ChannelType.VoiceChannel)
        {
            await _presenceRepository.DeleteVoiceChannel(message.ClanId, message.ChannelId);
            _logger.LogInformation("Ses kanalı varlığı ve katılımcıları temizlendi: {ChannelId}", message.ChannelId);
            
            // Bu mesajı alan frontend, eğer kullanıcı o kanaldaysa LiveKit bağlantısını koparacak.
            await _hubContext.Clients.Group($"clan_{message.ClanId}")
                .SendAsync("OnVoiceChannelDeleted", message.ChannelId);
        }
        else
        {
            // Metin kanalı silinme bildirimi
            await _hubContext.Clients.Group($"clan_{message.ClanId}")
                .SendAsync("OnChannelDeleted", message.ChannelId);
        }

        _logger.LogInformation("Kanal silinme bildirimi klan grubuna iletildi: {ClanId}", message.ClanId);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Kanal silme mesajı işlenirken hata oluştu: {ChannelId}", message.ChannelId);
        throw; // MassTransit'in hata yönetimi (retry) için throw ediyoruz.
    }
}
}
