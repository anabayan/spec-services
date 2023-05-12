using BuildingBlocks.Abstractions.Messaging;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Core.Messaging;

public class MessageBus : IBus
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<MessageBus> _logger;

    public MessageBus(
        ILogger<MessageBus> logger,
        DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public Task PublishEventAsync<TMessage>(
        string exchangeName,
        string topicName,
        TMessage message,
        Dictionary<string, string>? metadata = default,
        CancellationToken cancellationToken = default)
        where TMessage : class, IMessage
    {
        // Log the message
        _logger.LogInformation(
            "Publishing message of type {MessageType} to topic {TopicName} on exchange {ExchangeName} with metadata {Metadata}",
            message.GetType().Name,
            topicName,
            exchangeName,
            metadata);

        // Publish the message
        return _daprClient.PublishEventAsync(
            exchangeName,
            topicName,
            message,
            metadata,
            cancellationToken);
    }

    public Task PublishEventAsync<TMessage>(
        string exchangeName,
        string topicName,
        TMessage message,
        CancellationToken cancellationToken = default)
        where TMessage : class, IMessage
    {
        _logger.LogInformation(
            "Publishing message of type {MessageType} to topic {TopicName} on exchange {ExchangeName}",
            message.GetType().Name,
            topicName,
            exchangeName);

        return _daprClient.PublishEventAsync(
            exchangeName,
            topicName,
            message,
            cancellationToken);
    }
}
