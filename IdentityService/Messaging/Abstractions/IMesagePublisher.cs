using System;

namespace IdentityService.Messaging.Abstractions;

public interface IMesagePublisher
{
    public void Publish<T>(T message);
}
