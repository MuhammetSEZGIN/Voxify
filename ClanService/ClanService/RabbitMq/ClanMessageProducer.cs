using Shared.Contracts;
using ClanService.Interfaces.Services;
using MassTransit;

namespace ClanService.RabbitMq;

public class ClanMessageProducer : IClanMessageProducer
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ClanMessageProducer> _logger;
    public ClanMessageProducer(IPublishEndpoint publishEndpoint, ILogger<ClanMessageProducer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishChannelDeletedMessageAsync(
        string channelId,
        string clanId,
        ChannelType channelType
    )
    {
        var message = new ChannelDeletedMessage
        {
            ChannelId = channelId,
            ClanId = clanId,
            ChannelType = channelType
        };

        _logger.LogInformation(
            "Publishing VoiceChannelDeletedMessage for channel: {channelId}, clan: {clanId}",
            channelId,
            clanId
        );
        await _publishEndpoint.Publish(message);
    }
}
