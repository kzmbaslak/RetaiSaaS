using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Retail.Shared.Abstruction.Messaging;

namespace Retail.Catalog.Infrastructure.Messaging;

public sealed class RabbitMQEventBus : IEventBus, IDisposable
{

    private readonly IConnection _conn;
    private readonly IChannel _ch;
    private readonly string _exchangeName;
    private readonly JsonSerializerOptions _json;
    public RabbitMQEventBus(string host, int port = 5672, string user = "guest", string pass = "guest", string exchange = "retail.events")
    {
        var factory = new ConnectionFactory { HostName = host, Port = port, UserName = user, Password = pass};
        _conn = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _ch = _conn.CreateChannelAsync().GetAwaiter().GetResult();
        _exchangeName = exchange;
        _ch.ExchangeDeclareAsync(exchange: _exchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null); //Create exchange if not exists

        _json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public void Dispose()
    {
        try
        {
            _ch.CloseAsync();
            _conn.CloseAsync();
        }
        catch
        {
            //log
        }
        _ch.Dispose();
        _conn.Dispose();
    }

    public Task PublishAsync<T>(T message, string? topic = null, CancellationToken ct = default) where T : class
    {
        topic ??= typeof(T).Name;
        var payload = JsonSerializer.SerializeToUtf8Bytes(message, _json);
        var props = new BasicProperties
        {
            ContentType = "application/json",
            Persistent = true,
            MessageId = Guid.NewGuid().ToString()
        };

        _ch.BasicPublishAsync(exchange: _exchangeName, routingKey: topic, mandatory: false, basicProperties: props, body: payload, cancellationToken: ct);
        return Task.CompletedTask;
    }

    public Task SubscribeAsync<T>(string subscription, Func<T, Task> handler, string? topic = null, CancellationToken ct = default) where T : class
    {
        topic ??= typeof(T).Name;

        // Her subscription için kalıcı sıra
        var queueName = $"q.{subscription}.{topic}";
        _ch.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: ct);// Create queue if not exists
        _ch.QueueBindAsync(queue: queueName, exchange: _exchangeName, routingKey: topic, cancellationToken: ct); //bind queue to exchange with topic

        var consumer = new AsyncEventingBasicConsumer(_ch);
        consumer.ReceivedAsync +=  async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var msg = JsonSerializer.Deserialize<T>(body, _json);
            if (msg is not null)
            {
                await handler(msg);
                await _ch.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            else
            {
                //log
                await _ch.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };
        _ch.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: ct);// start consuming
        return Task.CompletedTask;
    }
}