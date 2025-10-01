using MediatR;
using Retail.BuildingBlocks.Primitives;
using Retail.Catalog.Domain.Aggregates.ProductAggregate;

namespace Retail.Catalog.Application.Products.Commands.Create;

public sealed record CreateProductCommand(
    Guid TenantId,
    string Name,
    string Sku,
    decimal? ListPrice,
    IEnumerable<string> Barcodes
) : IRequest<Result<ProductId>>;