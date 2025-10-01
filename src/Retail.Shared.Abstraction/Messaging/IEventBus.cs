namespace Retail.Shared.Abstruction.Messaging;

public interface IEventBus
{
    Task PublishAsync<T>(T message, string? topic = null, CancellationToken ct = default) where T : class;
    Task SubscribeAsync<T>(string subscription, Func<T, Task> handler, string? topic = null, CancellationToken ct = default) where T : class;
}
