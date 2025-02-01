using System;

namespace IdentityService.Messaging.Abstractions;

public interface IMessageConsumer
{

    public void Consume();
}
