using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Retail.Catalog.Infrastructure.Persistence.Outbox;
using Retail.Shared.Abstruction.Messaging;
namespace Retail.Catalog.Infrastructure.Persistence.BackgroundServices.BackgroundServices;

public sealed class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventPublisher _eventPublisher;
    public OutboxPublisher(IServiceProvider serviceProvider, IEventPublisher eventPublisher)
    {
        _serviceProvider = serviceProvider;
        _eventPublisher = eventPublisher;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

                var batch = await db.Set<OutboxMessage>()
                    .Where(o => o.ProcessedAtUtc == null)
                    .OrderBy(o => o.OccurredAtUtc)
                    .Take(100)
                    .ToListAsync();
                if (batch.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }
                foreach (var message in batch)
                {
                    try
                    {

                        var eventType = Type.GetType(message.Type, throwOnError: true)!;
                        var ie = JsonSerializer.Deserialize(message.Payload, eventType!);
                        if (ie is null) throw new InvalidOperationException("Deserialization returned null.");

                        await _eventPublisher.PublishAsync(ie, topic: eventType.Name, stoppingToken);
                        message.ProcessedAtUtc = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        message.Error = ex.Message;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                }
                await db.SaveChangesAsync(stoppingToken);
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}