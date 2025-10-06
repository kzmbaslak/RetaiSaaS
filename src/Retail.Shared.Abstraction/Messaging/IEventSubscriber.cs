namespace Retail.Shared.Abstruction.Messaging;

public interface IEventSubscriber
{
    Task SubscribeAsync<T>(string subscription, Func<T, Task> handler, string? topic = null, CancellationToken ct = default) where T : class;
}