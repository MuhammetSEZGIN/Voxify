using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace IdentityServiceTests.Mocks;

public class MockPublishEndpoint : IPublishEndpoint
{
    public List<object> PublishedMessages { get; } = new();

    public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
    {
        return null!;
    }

    public Task Publish<T>(T message, CancellationToken cancellationToken = default)
        where T : class
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task Publish<T>(
        T message,
        IPipe<PublishContext<T>> publishPipe,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task Publish<T>(
        T message,
        IPipe<PublishContext> publishPipe,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task Publish(object message, CancellationToken cancellationToken = default)
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task Publish(
        object message,
        IPipe<PublishContext> publishPipe,
        CancellationToken cancellationToken = default
    )
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task Publish(
        object message,
        Type messageType,
        CancellationToken cancellationToken = default
    )
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task Publish(
        object message,
        Type messageType,
        IPipe<PublishContext> publishPipe,
        CancellationToken cancellationToken = default
    )
    {
        PublishedMessages.Add(message);
        return Task.CompletedTask;
    }

    public Task Publish<T>(object values, CancellationToken cancellationToken = default)
        where T : class
    {
        return Task.CompletedTask;
    }

    public Task Publish<T>(
        object values,
        IPipe<PublishContext<T>> publishPipe,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        return Task.CompletedTask;
    }

    public Task Publish<T>(
        object values,
        IPipe<PublishContext> publishPipe,
        CancellationToken cancellationToken = default
    )
        where T : class
    {
        return Task.CompletedTask;
    }

    public void Clear()
    {
        PublishedMessages.Clear();
    }
}
