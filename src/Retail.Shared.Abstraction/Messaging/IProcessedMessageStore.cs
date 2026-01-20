namespace Retail.Shared.Abstruction.Messaging;

public interface IProcessedMessageStore
{
    Task<bool> HasProcessedAsync(string messageId, CancellationToken ct = default);
    Task MarkAsProcessedAsync(string messageId, DateTime processedAtUtc, CancellationToken ct = default);
}