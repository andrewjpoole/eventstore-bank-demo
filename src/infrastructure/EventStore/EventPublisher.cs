using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using EventStore.Client;

namespace Infrastructure.EventStore;

public class EventPublisher : IEventPublisher
{
    private readonly IEventStoreClientFactory _eventStoreClientFactory;

    public EventPublisher(IEventStoreClientFactory eventStoreClientFactory)
    {
        _eventStoreClientFactory = eventStoreClientFactory;
    }

    public async Task<bool> Publish<T>(T data, string streamName, CancellationToken cancellationToken)
    {
        _ = data ?? throw new ArgumentNullException(paramName: nameof(data));

        var metaData = data is IEvent @event ? new { Version = @event.Version() } : null;

        var client = _eventStoreClientFactory.CreateClient();

        var eventPayload = new EventData(
            eventId: Uuid.NewUuid(),
            type: typeof(T).Name,
            data: JsonSerializer.SerializeToUtf8Bytes(data),
            metadata: JsonSerializer.SerializeToUtf8Bytes(metaData)
        );
        var result = await client.AppendToStreamAsync(streamName, StreamState.Any, new[] { eventPayload }, cancellationToken: cancellationToken);

        return true;
    }

    public async Task<bool> Publish<T>(T data, string streamName, StreamRevision streamRevision, CancellationToken cancellationToken)
    {
        _ = data ?? throw new ArgumentNullException(paramName: nameof(data));

        var metaData = data is IEvent @event ? new { Version = @event.Version() } : null;

        var client = _eventStoreClientFactory.CreateClient();

        var eventPayload = new EventData(
            eventId: Uuid.NewUuid(),
            type: typeof(T).Name,
            data: JsonSerializer.SerializeToUtf8Bytes(data),
            metadata: JsonSerializer.SerializeToUtf8Bytes(metaData)
        );
        await client.AppendToStreamAsync(streamName, streamRevision, new[] { eventPayload }, cancellationToken: cancellationToken);

        return true;
    }
}