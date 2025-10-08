using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Retail.Shared.Abstruction.Messaging;

namespace Retail.Catalog.Infrastructure.Messaging.RabbitMq;

public sealed class RabbitMqSubscriber : IEventSubscriber
{
    private readonly RabbitMqConnection _conn;
    private readonly IMessageSerializer _serializer;
    private readonly IRoutingKeyResolver _routing;
    public RabbitMqSubscriber(RabbitMqConnection conn, IMessageSerializer serializer, IRoutingKeyResolver routingKeyResolver)
    {
        _conn = conn;
        _serializer = serializer;
        _routing = routingKeyResolver;
    }

    public async Task SubscribeAsync<T>(string subscription, Func<T, Task> handler, string? topic = null, CancellationToken ct = default) where T : class
    {
        var routingKey = _routing.ResolveFor<T>(topic);
        var queueName = $"q.{subscription}.{routingKey}";

        await _conn.Channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        await _conn.Channel.QueueBindAsync(queue: queueName, exchange: _conn.Exchange, routingKey: routingKey);

        var consumer = new AsyncEventingBasicConsumer(_conn.Channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var msg = _serializer.Deserialize<T>(ea.Body.ToArray());
                if (msg is null) throw new InvalidOperationException("Deserialization returned null.");
                await handler(msg);
                await _conn.Channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch
            {
                await _conn.Channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        await _conn.Channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
    }
}