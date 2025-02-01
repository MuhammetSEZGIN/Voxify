using System;
using IdentityService.Messaging.Abstractions;

namespace IdentityService.Messaging.RabbitMQ;

public class RabbitMqPublisher : IMesagePublisher

{
    public void Publish<T>(T message)
    {
        throw new NotImplementedException();
    }
}

