using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using EventStore.Client;

namespace Infrastructure.EventStore;

public class EventPublisher : IEventPublisher
{
    private readonly IEventStoreClientFactory _eventStoreClientFactory;

    public EventPublisher(IEventStoreClientFactory eventStoreClientFactory)
    {
        _eventStoreClientFactory = eventStoreClientFactory;
    }
    
    private static (string TypeNameWithoutVersion, string Version) GetNameAndVersion(Type type)
    {
        var fullName = type.Name;
        
        var underscorePosition = fullName.IndexOf("_", StringComparison.Ordinal);
        if (underscorePosition < 1)
            throw new NotSupportedException("Expected event type name to contain an underscore followed by a version number");

        var typeNameWithoutVersion = fullName.Substring(0, underscorePosition);

        var version = fullName.Substring(underscorePosition);

        return (typeNameWithoutVersion, version);
    }

    public async Task<bool> Publish<T>(T data, string streamName, CancellationToken cancellationToken) where T : IEvent
    {
        _ = data ?? throw new ArgumentNullException(paramName: nameof(data));

        var isValid = data.IsValid();
        if (isValid.IsT1)
            throw new InvalidOperationException($"Can't publish an invalid event. Type:{typeof(T).Name}, Errors:{string.Join(",", isValid.AsT1)}");

        var (typeNameWithoutVersion, version) = GetNameAndVersion(typeof(T));

        var metaData = data is IEvent @event ? new { Version = version } : null;

        var client = _eventStoreClientFactory.CreateClient();

        var eventPayload = new EventData(
            eventId: Uuid.NewUuid(),
            type: typeNameWithoutVersion,
            data: JsonSerializer.SerializeToUtf8Bytes(data),
            metadata: JsonSerializer.SerializeToUtf8Bytes(metaData)
        );
        await client.AppendToStreamAsync(streamName, StreamState.Any, new[] { eventPayload }, cancellationToken: cancellationToken);

        return true;
    }

    public async Task<bool> Publish<T>(T data, string streamName, long expectedPosition, CancellationToken cancellationToken) where T : IEvent
    {
        _ = data ?? throw new ArgumentNullException(paramName: nameof(data));

        var isValid = data.IsValid();
        if (isValid.IsT0)
            throw new InvalidOperationException($"Can't publish an invalid event. Type:{typeof(T).Name}, Errors:{string.Join(",", isValid.AsT1)}");

        var (typeNameWithoutVersion, version) = GetNameAndVersion(typeof(T));

        var metaData = data is IEvent @event ? new { Version = version } : null;

        var client = _eventStoreClientFactory.CreateClient();

        var eventPayload = new EventData(
            eventId: Uuid.NewUuid(),
            type: typeNameWithoutVersion,
            data: JsonSerializer.SerializeToUtf8Bytes(data),
            metadata: JsonSerializer.SerializeToUtf8Bytes(metaData)
        );

        var streamRevision = StreamRevision.FromStreamPosition(StreamPosition.FromInt64(expectedPosition));

        await client.AppendToStreamAsync(streamName, streamRevision, new[] { eventPayload }, cancellationToken: cancellationToken);

        return true;
    }
}