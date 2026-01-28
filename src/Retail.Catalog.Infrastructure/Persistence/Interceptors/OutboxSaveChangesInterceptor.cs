using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Retail.BuildingBlocks.Domain;
using Retail.Catalog.Infrastructure.Persistence.Outbox;
using Retail.Shared.Abstraction.Messaging;

namespace Retail.Catalog.Infrastructure.Persistence.Interceptors;

public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    IEventMapper _eventMapper;
    JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    public OutboxSaveChangesInterceptor(IEventMapper eventMapper)
    {
        _eventMapper = eventMapper;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        EnqueueOutbox(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    override public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        EnqueueOutbox(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        ClearDomainEvents(eventData.Context!);
        return base.SavedChanges(eventData, result);
    }

    private void ClearDomainEvents(DbContext dbContext)
    {
        var aggregateRoots = dbContext.ChangeTracker
            .Entries<AggregateRoot<object>>()
            .Select(e => e.Entity)
            .ToList();
        foreach (var aggregateRoot in aggregateRoots)
            aggregateRoot.ClearDomainEvents();
    }

    private void EnqueueOutbox(DbContext? context)
    {
        if (context is null) return;

        var outbox = context.Set<OutboxMessage>();

        // ChangeTracker üzerinden aggregate’lardan domain event’leri çek
        var domainEvents = context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is AggregateRoot<object>)
            .Select(e => e.Entity as dynamic)
            .SelectMany(entity =>
            {
                var events = ((IEnumerable<IDomainEvent>)entity.DequeueDomainEvents()).ToList();
                return events;
            }).ToList();

        foreach (var de in domainEvents)
        {
            foreach (var ie in _eventMapper.Map(de))
            {
                var payload = JsonSerializer.Serialize(ie, ie.GetType(), _jsonSerializerOptions);
                outbox.Add(new OutboxMessage
                {
                    Type = ie.GetType().FullName ?? string.Empty,
                    Payload = payload,
                    OccurredAtUtc = DateTime.UtcNow
                });
            }
        }
    }
}