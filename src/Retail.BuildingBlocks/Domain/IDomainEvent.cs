namespace Retail.BuildingBlocks.Domain;


public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAtUtc { get; }
}