namespace Retail.Shared.Abstruction.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T message, string? topic = null, CancellationToken ct = default) where T : class;
}