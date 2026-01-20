using MessagePack;
using MessagePack.Resolvers;
using Retail.Shared.Abstruction.Messaging;

public sealed class MessagePackMessageSerializer : IMessageSerializer
{
    private MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard
        .WithResolver(ContractlessStandardResolver.Instance) // Class'lara [Key] attribute'u eklemek zorunda kalmamak için
        .WithCompression(MessagePackCompression.Lz4BlockArray); // Sıkıştırma için
    public string ContentType => "application/x-msgpack";

    public T Deserialize<T>(byte[] payload) where T : class
    {
        return MessagePackSerializer.Deserialize<T>(payload, _options);
    }

    public byte[] Serialize<T>(T message) where T : class
    {
        return MessagePackSerializer.Serialize(message, _options);
    }

}