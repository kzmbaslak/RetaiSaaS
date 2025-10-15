namespace Retail.Catalog.Infrastructure.Messaging;

public interface IEventMapper
{
    // Domain event -> Integration event (0..n)
    IEnumerable<IIntegrationEvent> Map(object domainEvent);
}