using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using Retail.Catalog.Infrastructure.Messaging.Configuration;

namespace Retail.Catalog.Infrastructure.Messaging.RabbitMq;

public sealed class RabbitMqConnection : IDisposable
{
    public IConnection Connection { get; }
    public IChannel Channel { get; }
    public string Exchange { get; }
    public RabbitMqConnection(RabbitMqOptions options)
    {
        var factory = new ConnectionFactory()
        {
            HostName = options.Host,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
        };

        Connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        Channel = Connection.CreateChannelAsync().GetAwaiter().GetResult();
        Exchange = options.Exchange;

        Channel.ExchangeDeclareAsync(exchange: Exchange, type: options.ExchangeType, durable: true, autoDelete: false, arguments: null); //Create exchange if not exists)  
    }
    public void Dispose()
    {
        try { if (Channel.IsOpen) Channel.Close(); } catch { }
        try { if (Connection.IsOpen) Connection.Close(); } catch { }
        Channel.Dispose();
        Connection.Dispose();
    }
}