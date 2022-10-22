using System;
using System.Text.Json;
using Domain;

namespace Infrastructure.EventStore.Serialisation;

public class EventDeserialiser : IEventDeserialiser
{
    private readonly IDeserialisationTypeMapper _typeMapper;

    public EventDeserialiser(IDeserialisationTypeMapper typeMapper)
    {
        _typeMapper = typeMapper;
    }

    public IEvent DeserialiseEvent(string typeName, string version, string json)
    {
        // pass in the version from the event metadata and construct the typename inc '_'
        var typeNameWithVersion = $"{typeName}{version}";

        var type = _typeMapper.GetTypeFromName(typeNameWithVersion);

        var @event = JsonSerializer.Deserialize(json, type,
                         new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
                     throw new InvalidOperationException($"Can't Deserialise type {typeNameWithVersion}");

        return (IEvent)@event;
    }

    public IEvent DeserialiseEvent(IEventWrapper eventWrapper)
    {
        return DeserialiseEvent(eventWrapper.EventTypeName, eventWrapper.Version, eventWrapper.EventJson);
    }
}