using Retail.Shared.Abstruction.Messaging;

namespace Retail.Shared.Abstraction.Messaging;

public interface IEventMapper
{
    // Domain event -> Integration event (0..n)
    IEnumerable<IIntegrationEvent> Map(object domainEvent);
}