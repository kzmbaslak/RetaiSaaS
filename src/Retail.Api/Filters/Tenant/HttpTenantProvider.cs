

using Retail.Shared.Abstractions.MultiTenancy;

namespace Retail.Api.Filters.Tenant;

public sealed class HttpTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _http;
    public HttpTenantProvider(IHttpContextAccessor http)
    {
        _http = http;
    }
    
    public Guid CurrentTenantId
    {
        get
        {
            
            var ctx = _http.HttpContext;
            if (ctx != null && ctx.Request.Headers.TryGetValue("X-Tenant-Id", out var id) && Guid.TryParse(id, out var tenantId))
                return tenantId;
            return Guid.Empty;
        }
    }

}