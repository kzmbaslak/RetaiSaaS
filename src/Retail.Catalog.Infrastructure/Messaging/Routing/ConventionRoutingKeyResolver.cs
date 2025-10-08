using Retail.Shared.Abstruction.Messaging;

namespace Retail.Catalog.Infrastructure.Messaging.Routing;

public sealed class ConventionRoutingKeyResolver : IRoutingKeyResolver
{
    public string ResolveFor<T>(string? topicOverride = null)
    {
        return string.IsNullOrWhiteSpace(topicOverride) ? typeof(T).Name :topicOverride!;
    }
}