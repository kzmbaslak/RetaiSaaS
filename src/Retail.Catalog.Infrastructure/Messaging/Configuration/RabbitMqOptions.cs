namespace Retail.Catalog.Infrastructure.Messaging.Configuration;

public sealed class RabbitMqOptions
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string Exchange { get; init; } = "retail.events";
    public string ExchangeType { get; init; } = "topic";
}