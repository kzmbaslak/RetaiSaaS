using Retail.Shared.Abstruction.Messaging;

namespace Retail.Api.Workers;

public class ProductCreatedSubscriber : BackgroundService
{
    private readonly IEventSubscriber _eventSubscriber;
    private readonly IProcessedMessageStore _store;
    public ProductCreatedSubscriber(IEventSubscriber eventSubscriber, IProcessedMessageStore processedMessageStore)
    {
        _eventSubscriber = eventSubscriber;
        _store = processedMessageStore;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _eventSubscriber.SubscribeAsync<ProductCreatedIntegrationEvent>(
            subscription: "api-reporting",
            handler: async (evt) =>
            {
                if (await _store.HasProcessedAsync(evt.MessageId.ToString(), stoppingToken))
                {
                    return;
                }

                Console.WriteLine($"[SUB] Product created: {evt.Sku} - {evt.Name}");


                await _store.MarkAsProcessedAsync(evt.MessageId.ToString(), DateTime.UtcNow, stoppingToken);
            },
            topic: "catalog.product.created",
            ct: stoppingToken
        );
    }
}
public sealed record ProductCreatedIntegrationEvent(Guid MessageId, Guid TenantId, string Sku, string Name);
