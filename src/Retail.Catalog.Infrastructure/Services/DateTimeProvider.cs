using Retail.Shared.Abstractions.Time;

namespace Retail.Catalog.Infrastructure.Service;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}