using Microsoft.EntityFrameworkCore;
using Retail.Catalog.Infrastructure.Persistence;
using Retail.Shared.Abstruction.Messaging;

public sealed class PostgresProcessedMessageStore : IProcessedMessageStore
{
    private readonly CatalogDbContext _db;
    public PostgresProcessedMessageStore(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<bool> HasProcessedAsync(string messageId, CancellationToken ct = default)
    {
        return await _db.Set<ProcessedMessage>()
            .AnyAsync(pm => pm.MessageId == messageId, ct);
    }

    public async Task MarkAsProcessedAsync(string messageId, DateTime processedAtUtc, CancellationToken ct = default)
    {
        await _db.Set<ProcessedMessage>()
            .AddAsync(new ProcessedMessage
            {
                MessageId = messageId,
                ProcessedAt = processedAtUtc
            }, ct);
        await _db.SaveChangesAsync(ct);
    }
}

