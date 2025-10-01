using Retail.Shared.Abstruction.Messaging;

namespace Retail.Api.Workers;

public class ProductCreatedSubscriber : BackgroundService
{
    private readonly IEventBus _eventBus;

    public ProductCreatedSubscriber(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _eventBus.SubscribeAsync<ProductCreatedIntegrationEvent>(
            subscription: "api-reporting",
            handler: async (evt) =>
            {
                Console.WriteLine($"[SUB] Product created: {evt.Sku} - {evt.Name}");
                await Task.CompletedTask;
            },
            topic: "catalog.product.created",
            ct: stoppingToken
        );
    }
}
public sealed record ProductCreatedIntegrationEvent(Guid ProductId, Guid TenantId, string Sku, string Name);
