namespace BuildingBlocks.Abstractions.Messaging;

public interface IBus
{
    public Task PublishEventAsync<TMessage>(string exchangeName,
        string topicName,
        TMessage message,
        Dictionary<string, string>? metadata = default,
        CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;

    public Task PublishEventAsync<TMessage>(
        string exchangeName,
        string topicName,
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;
}
