using MediatR;
using Retail.BuildingBlocks.Primitives;
using Retail.Catalog.Domain.Aggregates.ProductAggregate;
using Retail.Catalog.Domain.Repositories;
using Retail.Shared.Abstruction.Messaging;

namespace Retail.Catalog.Application.Products.Commands.Create;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductId>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventPublisher _eventPublisher;
    public CreateProductCommandHandler(IProductRepository productRepository, IEventPublisher eventPublisher)
    {
        _productRepository = productRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<ProductId>> Handle(CreateProductCommand req, CancellationToken ct)
    {
        if (await _productRepository.ExistsSkuAsync(req.TenantId, req.Sku, ct))
        {
            return Result<ProductId>.Fail("sku conflict", $"SKU({req.Sku}) already exists for tenant");
        }

        var product = Product.Create(req.TenantId, req.Name, req.Sku, req.ListPrice, req.Barcodes);

        await _productRepository.AddAsync(product, ct);

        // Basit integration event (DTO)
        var evt = new ProductCreatedIntegrationEvent(product.Id.Value, product.TenantId, product.Sku, product.Name);
        await _eventPublisher.PublishAsync(evt, topic: "catalog.product.created", ct);

        return Result<ProductId>.Ok(product.Id);
    }
}

// DTO'yu Application içinde tutuyoruz (transport model)
public sealed record ProductCreatedIntegrationEvent(Guid ProductId, Guid TenantId, string Sku, string Name);