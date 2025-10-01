using Microsoft.EntityFrameworkCore;
using Retail.Catalog.Domain.Aggregates.ProductAggregate;
using Retail.Shared.Abstractions.MultiTenancy;

namespace Retail.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext : DbContext
{
    private readonly ITenantProvider _tenant;

    public DbSet<Product> Products => Set<Product>();
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options, ITenantProvider tenant) : base(options)
    {
        _tenant = tenant;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);//EF Core, bu assembly’de IEntityTypeConfiguration<T> arayüzünü implement eden tüm sınıfları yansıma (reflection) ile tarar.

        //Global Tenant Filtresi
        modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == _tenant.CurrentTenantId);
    }
}