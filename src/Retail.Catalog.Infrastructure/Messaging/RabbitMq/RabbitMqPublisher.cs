using RabbitMQ.Client;
using Retail.Shared.Abstruction.Messaging;

namespace Retail.Catalog.Infrastructure.Messaging.RabbitMq;

public sealed class RabbitMqPublisher : IEventPublisher
{
    private readonly RabbitMqConnection _conn;
    private readonly IMessageSerializer _serializer;
    private readonly IRoutingKeyResolver _routingKeyResolver;


    public RabbitMqPublisher(RabbitMqConnection conn, IMessageSerializer serializer, IRoutingKeyResolver routingKeyResolver)
    {
        _conn = conn;
        _serializer = serializer;
        _routingKeyResolver = routingKeyResolver;
    }
    public Task PublishAsync<T>(T message, string? topic = null, CancellationToken ct = default) where T : class
    {
        var routingKey = _routingKeyResolver.ResolveFor<T>(topic);
        var payload = _serializer.Serialize(message);
        var props = new BasicProperties
        {
            ContentType = _serializer.ContentType,
            Persistent = true,
            MessageId = Guid.NewGuid().ToString()
        };

        _conn.Channel.BasicPublishAsync(exchange: _conn.Exchange, routingKey: routingKey, mandatory: false, basicProperties: props, body: payload, cancellationToken: ct);
        return Task.CompletedTask;
    }
}