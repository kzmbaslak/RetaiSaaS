namespace Retail.Shared.Abstruction.Messaging;

public interface IMessageSerializer
{
    byte[] Serialize<T>(T message) where T : class;
    T Deserialize<T>(byte[] payload) where T : class;
    string ContentType { get; }
}