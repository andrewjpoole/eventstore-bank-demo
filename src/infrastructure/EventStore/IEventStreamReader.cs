using System.Collections.Generic;
using System.Threading.Tasks;
using events;

namespace infrastructure.EventStore
{
    public interface IEventStreamReader<T> where T : IEvent
    {
        Task<IEnumerable<(T EventData, EventMetadata EventMetadata)>> ReadEventsFromStream(string streamName);
    }
}