using Retail.Catalog.Domain.Aggregates.ProductAggregate;

namespace Retail.Catalog.Domain.Repositories;

public interface IProductRepository
{
    Task<bool> ExistsSkuAsync(Guid tenantId, string sku, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task<Product?> GetAsync(ProductId id, CancellationToken ct);
}