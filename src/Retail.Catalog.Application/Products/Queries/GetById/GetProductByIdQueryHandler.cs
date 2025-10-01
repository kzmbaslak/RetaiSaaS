using MediatR;
using Retail.BuildingBlocks.Primitives;
using Retail.Catalog.Domain.Aggregates.ProductAggregate;
using Retail.Catalog.Domain.Repositories;

namespace Retail.Catalog.Application.Products.Queries.GetById;

public sealed record GetProductByIdQueryHandler: IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery req, CancellationToken ct)
    {
        var p = await _productRepository.GetAsync(new ProductId(req.ProductId), ct);
        if (p is null || p.TenantId != req.TenantId)
            return Result<ProductDto>.Fail("not_found", "Product not found");

        var dto = new ProductDto(p.Id.Value, p.Name, p.Sku, p.ListPrice, p.Barcodes.Select(b => b.Value).ToArray());
        return Result<ProductDto>.Ok(dto);
    }
}