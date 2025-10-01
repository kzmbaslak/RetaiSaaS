namespace Retail.Shared.Abstractions.MultiTenancy;

public interface ITenantProvider
{
    Guid CurrentTenantId { get; }
}