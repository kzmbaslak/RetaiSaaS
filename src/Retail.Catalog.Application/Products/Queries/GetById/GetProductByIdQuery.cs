using MediatR;
using Retail.BuildingBlocks.Primitives;

namespace Retail.Catalog.Application.Products.Queries.GetById;

public sealed record GetProductByIdQuery(Guid TenantId, Guid ProductId) : IRequest<Result<ProductDto>>;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Sku,
    decimal? ListPrice,
    IEnumerable<string> Barcodes);