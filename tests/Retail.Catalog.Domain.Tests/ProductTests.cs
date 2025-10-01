using FluentAssertions;
using Retail.Catalog.Domain.Aggregates.ProductAggregate;

namespace Retail.Catalog.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void AddBarcode_ShouldBeIdempotent()
    {
        var p = Product.Create(Guid.NewGuid(), "Çay", "CAY-001", 100m);
        p.AddBarcode(new Barcode("123456789012"));
        p.AddBarcode(new Barcode("123456789012"));
        p.Barcodes.Should().HaveCount(1);

    }
}