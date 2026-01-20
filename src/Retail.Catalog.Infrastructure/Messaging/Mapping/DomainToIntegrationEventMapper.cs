using Retail.Catalog.Domain.Aggregates.ProductAggregate.Events;
using Retail.Shared.Abstraction.Messaging;
using Retail.Shared.Abstruction.Messaging;

namespace Retail.Catalog.Infrastructure.Messaging.Mapping;

public class DomainToIntegrationEventMapper : IEventMapper
{
    public IEnumerable<IIntegrationEvent> Map(object domainEvent)
    {
        switch (domainEvent)
        {
            case ProductCreatedEvent e:
                yield return new ProductCreatedIntegrationEvent(
                    MessageId: Guid.NewGuid().ToString(),
                    OccurredAtUtc: e.OccurredAtUtc,
                    ProductId: e.ProductId.Value,
                    TenantId: e.TenantId);
                break;
            default:
                yield break;
        }
    }

    public sealed record ProductCreatedIntegrationEvent(
        string MessageId,
        DateTime OccurredAtUtc,
        Guid ProductId,
        Guid TenantId
    ) : IIntegrationEvent;
}