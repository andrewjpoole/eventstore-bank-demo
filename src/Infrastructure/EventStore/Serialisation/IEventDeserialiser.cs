using Domain.Interfaces;

namespace Infrastructure.EventStore.Serialisation;

public interface IEventDeserialiser
{
    IEvent DeserialiseEvent(string typeName, string version, string json);
    IEvent DeserialiseEvent(IEventWrapper eventWrapper);
}