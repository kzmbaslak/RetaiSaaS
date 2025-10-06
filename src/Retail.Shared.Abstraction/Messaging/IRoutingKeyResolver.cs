namespace Retail.Shared.Abstruction.Messaging;

public interface IRoutingKeyResolver
{
    string ResolveFor<T>(string? topicOverride = null);
}