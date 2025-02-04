using System;

namespace IdentityService.Interfaces;

public interface IMessagePublisher
{
    public Task PublishAsync<T>(T message);
}
