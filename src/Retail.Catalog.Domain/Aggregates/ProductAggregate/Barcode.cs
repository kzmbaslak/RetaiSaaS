namespace Retail.Catalog.Domain.Aggregates.ProductAggregate;

public sealed class Barcode : IEquatable<Barcode>
{
    public string Value { get; }
    public Barcode() => Value = string.Empty;
    public Barcode(string value)
    {
        Value = value?.Trim() ?? throw new ArgumentNullException(nameof(value));
        if (value.Length is < 3 or > 64)
            throw new ArgumentOutOfRangeException(nameof(value), "Barcode length must be between 3 and 64 characters.");
    }
    public bool Equals(Barcode? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Barcode barcode && Equals(barcode);

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

}
