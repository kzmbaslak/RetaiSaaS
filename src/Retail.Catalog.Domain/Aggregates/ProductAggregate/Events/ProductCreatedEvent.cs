namespace Retail.Catalog.Domain.Aggregates.ProductAggregate.Events;

using Retail.BuildingBlocks.Domain;

public sealed record ProductCreatedEvent(ProductId ProductId, Guid TenantId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}