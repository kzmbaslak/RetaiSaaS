using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Retail.Catalog.Domain.Aggregates.ProductAggregate;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");


        var idConv = new ValueConverter<ProductId, Guid>( //ProductId <-> Guid
            v => v.Value,
            v => new ProductId(v)
        );

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion(idConv);
        builder.Property(p => p.TenantId).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Sku).HasMaxLength(100).IsRequired();
        builder.Property(p => p.ListPrice);

        builder.HasIndex(p => new { p.TenantId, p.Sku }).IsUnique();

        builder.OwnsMany(p => p.Barcodes, b =>
        {
            b.ToTable("product_barcodes");
            b.WithOwner().HasForeignKey("ProductId");
            b.Property<int>("Id");
            b.HasKey("Id");
            b.Property(b => b.Value).HasColumnName("code").HasMaxLength(64).IsRequired();
            b.HasIndex(b => b.Value);
        });
    }
}