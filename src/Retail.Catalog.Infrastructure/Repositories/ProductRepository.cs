using Microsoft.EntityFrameworkCore;
using Retail.Catalog.Domain.Aggregates.ProductAggregate;
using Retail.Catalog.Domain.Repositories;
using Retail.Catalog.Infrastructure.Persistence;

namespace Retail.Catalog.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _db;
    public ProductRepository(CatalogDbContext db) => _db = db;

    public async Task AddAsync(Product product, CancellationToken ct)
    {

        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync(ct);
    }

    public Task<bool> ExistsSkuAsync(Guid tenantId, string sku, CancellationToken ct)
    {
        return _db.Products.IgnoreQueryFilters().AnyAsync(p => p.TenantId == tenantId && p.Sku == sku, ct);
    }

    public Task<Product?> GetAsync(ProductId id, CancellationToken ct)
    {
        return _db.Products.Include(p => p.Barcodes).FirstOrDefaultAsync(p => p.Id == id, ct);

    }
}