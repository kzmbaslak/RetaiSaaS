internal sealed class ProcessedMessage
{
    public Guid Id { get; set; }
    public string MessageId { get; set; } = default!;
    public DateTime ProcessedAt { get; set; }
}