using System;
using ClanService.DTOs;
using ClanService.Interfaces.Services;
using MassTransit;

namespace ClanService.RabbitMq;

public class ClanServicePublisher : IClanServicePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<IClanServicePublisher> _logger;

    public ClanServicePublisher(IPublishEndpoint publishEndpoint, ILogger<IClanServicePublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishDeleteChannelMessageAsync(ChannelDeletedMessage message)
    {
        if(message.ChannelId == Guid.Empty)
        {
            _logger.LogError("ChannelId is empty.");
            return;
        }

        var messageToPublish = new ChannelDeletedMessage
        {
           ChannelId = message.ChannelId
        };

        _logger.LogInformation("Publishing ChannelDeletedMessage with ChannelId: {ChannelId}", message.ChannelId);
        await _publishEndpoint.Publish(messageToPublish);
    }
}
