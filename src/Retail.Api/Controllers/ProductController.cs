

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Retail.BuildingBlocks.Primitives;
using Retail.Catalog.Application.Products.Commands.Create;
using Retail.Catalog.Application.Products.Queries.GetById;

namespace Retail.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(new { id = result.Value }) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById([FromRoute] Guid id)
    {
        if (!Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdString) || !Guid.TryParse(tenantIdString.FirstOrDefault(), out var tenantId))
            return Unauthorized(new { error = "Missing X-Tenant-Id" });
        var res = _mediator.Send(new GetProductByIdQuery(tenantId, id));
        return res.Result.IsSuccess ? Ok(res.Result.Value) : NotFound(res.Result.Error);
    }
}