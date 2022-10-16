using Domain;

namespace Infrastructure.EventStore.Serialisation;

public interface IEventDeserialiser
{
    IEvent DeserialiseEvent(string typeName, string json);
}