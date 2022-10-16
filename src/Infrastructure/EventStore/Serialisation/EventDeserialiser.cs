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

    public IEvent DeserialiseEvent(string typeName, string json)
    {
        var type = _typeMapper.GetTypeFromName(typeName);

        var @event = JsonSerializer.Deserialize(json, type,
                         new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ??
                     throw new InvalidOperationException($"Can't Deserialise type {typeName}");

        return (IEvent)@event;
    }
}