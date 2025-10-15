namespace Retail.Catalog.Infrastructure.Messaging;

public interface IIntegrationEvent
{
    string MessageId { get; }
    DateTime OccurredAtUtc  { get; }
}