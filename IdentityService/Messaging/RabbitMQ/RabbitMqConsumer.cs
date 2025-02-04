using System;
using IdentityService.Interfaces;
using RabbitMQ.Client;

namespace IdentityService.Messaging.RabbitMQ;

public class RabbitMqConsumer : IMessageConsumer
{
    private readonly RabbitMqManager _rabbitMqManager;
    private readonly IChannel _channel;
    public RabbitMqConsumer(RabbitMqManager rabbitMqManager)
    {
        _rabbitMqManager = rabbitMqManager;
        _channel = _rabbitMqManager.CreateChannel().Result;
    }
    public void Consume()
    {
        
    }
}
