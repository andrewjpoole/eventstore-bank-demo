using System;
using System.Text;
using System.Text.Json;
using Domain;
using EventStore.Client;

namespace Infrastructure.EventStore.Serialisation;

public class EventWrapper : IEventWrapper
{
    public string EventJson { get; init; }
    public string EventTypeName { get; init; }
    public DateTime Created { get; init; }
    public Guid EventId { get; init; }
    public long EventNumber { get; init; }
    public string Version {
        get
        {
            metadataDynamic ??= JsonSerializer.Deserialize<dynamic>(metadataJson)
                                ?? throw new InvalidOperationException("Unable to load a dynamic from the metadata Json, please ensure its valid Json.");

            return metadataDynamic.Version;
        }
    }
    public dynamic Metadata {
        get
        {
            metadataDynamic ??= JsonSerializer.Deserialize<dynamic>(metadataJson)
                                ?? throw new InvalidOperationException("Unable to load a dynamic from the metadata Json, please ensure its valid Json.");

            return metadataDynamic;
        }
    }

    private readonly string metadataJson;
    private dynamic metadataDynamic;

    public EventWrapper(ResolvedEvent @event)
    {
        metadataJson = Encoding.UTF8.GetString(@event.OriginalEvent.Metadata.ToArray());

        Created = @event.Event.Created;
        EventId = @event.OriginalEvent.EventId.ToGuid();
        EventNumber = @event.OriginalEvent.EventNumber.ToInt64();
        
        EventJson = Encoding.UTF8.GetString(@event.OriginalEvent.Data.ToArray());
        EventTypeName = @event.OriginalEvent.EventType;
    }
}