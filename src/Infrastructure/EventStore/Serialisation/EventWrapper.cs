using System;
using System.Text;
using System.Text.Json;
using Domain.Interfaces;
using EventStore.Client;

namespace Infrastructure.EventStore.Serialisation;

public class EventWrapper : IEventWrapper
{
    public string EventJson { get; init; }
    public string EventTypeName { get; init; }
    public DateTime Created { get; init; }
    public Guid EventId { get; init; }
    public long EventNumber { get; init; }
    public string Version { get; init; }

    public dynamic Metadata => _metadata;

    private readonly JsonElement _metadata;

    public EventWrapper(ResolvedEvent resolvedEvent)
    {
        var @event = resolvedEvent.Event; // this seems to be the real event whether it is resolved or not

        var metadataJson = Encoding.UTF8.GetString(@event.Metadata.ToArray());
        _metadata = JsonSerializer.Deserialize<JsonElement>(metadataJson);
        
        Version = _metadata.GetProperty(nameof(Version)).GetString();

        Created = @event.Created;
        EventId = @event.EventId.ToGuid();
        EventNumber = @event.EventNumber.ToInt64();
        
        EventJson = Encoding.UTF8.GetString(@event.Data.ToArray());
        EventTypeName = @event.EventType;

        if (EventTypeName == "$>")
            throw new InvalidOperationException("Deserialisation will fail as the typename is $> for linked events");
    }
}

public static class EventExtensions
{
    public static string ToJson(this EventRecord @event)
    {
        return Encoding.UTF8.GetString(@event.Data.ToArray());
    }

    public static string ToMetadataJson(this EventRecord @event)
    {
        return Encoding.UTF8.GetString(@event.Metadata.ToArray());
    }
}