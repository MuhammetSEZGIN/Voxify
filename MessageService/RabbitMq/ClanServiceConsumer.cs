using System;
using Shared.Contracts;
using MassTransit;

namespace MessageService.RabbitMq;

public class ClanServiceConsumer : IConsumer<ChannelDeletedMessage>
{
    public Task Consume(ConsumeContext<ChannelDeletedMessage> context)
    {
        throw new NotImplementedException();
    }
}
