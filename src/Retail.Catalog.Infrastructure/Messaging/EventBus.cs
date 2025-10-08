using Retail.Shared.Abstruction.Messaging;

namespace Retail.Catalog.Infrastructure.Messaging;

public sealed class EventBus : IEventPublisher, IEventSubscriber
{
    IEventPublisher _publisher;
    IEventSubscriber _subscriber;
    public EventBus(IEventPublisher publisher, IEventSubscriber subscriber)
    {
        _publisher = publisher;
        _subscriber = subscriber;
    }

    public Task PublishAsync<T>(T message, string? topic = null, CancellationToken ct = default) where T : class
        => _publisher.PublishAsync(message, topic, ct);

    public Task SubscribeAsync<T>(string subscription, Func<T, Task> handler, string? topic = null, CancellationToken ct = default) where T : class
        => _subscriber.SubscribeAsync(subscription, handler, topic, ct);
}