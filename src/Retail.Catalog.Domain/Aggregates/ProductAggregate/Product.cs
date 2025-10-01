using Retail.BuildingBlocks.Domain;


namespace Retail.Catalog.Domain.Aggregates.ProductAggregate;


public sealed class Product : AggregateRoot<ProductId>
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Sku { get; private set; } = default!; // Stock Keeping Unit
    public decimal? ListPrice { get; private set; }


    private readonly List<Barcode> _barcodes = new();
    public IReadOnlyCollection<Barcode> Barcodes => _barcodes.AsReadOnly();


    private Product() { }


    public static Product Create(Guid tenantId, string name, string sku, decimal? listPrice = null, IEnumerable<string>? barcodes = null)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId required");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required");
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("Sku required");


        var p = new Product
        {
            Id = ProductId.New(),
            TenantId = tenantId,
            Name = name.Trim(),
            Sku = sku.Trim(),
            ListPrice = listPrice
        };
        if (barcodes is not null)
            foreach (var bc in barcodes.Distinct()) p.AddBarcode(new Barcode(bc));


        p.Raise(new Events.ProductCreatedEvent(p.Id, p.TenantId));
        return p;
    }


    public void AddBarcode(Barcode bc)
    {
        if (_barcodes.Any(x => x.Value == bc.Value)) return;
        _barcodes.Add(bc);
    }
}