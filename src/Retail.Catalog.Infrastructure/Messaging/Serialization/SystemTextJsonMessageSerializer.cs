using System.Text.Json;
using Retail.Shared.Abstruction.Messaging;

namespace Retail.Catalog.Infrastructure.Messaging.Serialization;

public sealed class SystemTextJsonMessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public string ContentType => "application/json";


    public byte[] Serialize<T>(T message) where T : class
    {
        return JsonSerializer.SerializeToUtf8Bytes(message, _json);
    }

    public T Deserialize<T>(byte[] payload) where T : class
    {
        return JsonSerializer.Deserialize<T>(payload, _json) ?? throw new InvalidOperationException("Deserialization resulted in null");
    }


}