namespace Retail.Shared.Abstractions.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}