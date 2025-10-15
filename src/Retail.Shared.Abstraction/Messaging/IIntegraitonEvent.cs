namespace Retail.Shared.Abstruction.Messaging;

public interface IIntegrationEvent
{
    string MessageId { get; }
    DateTime OccurredAtUtc  { get; }
}