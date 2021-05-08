using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using events;
using EventStore.ClientAPI;

namespace infrastructure.EventStore
{
    public class EventStreamReader<T> : IEventStreamReader<T> where T : IEvent
    {
        private readonly IEventStoreConnectionFactory _eventStoreConnectionFactory;

        public EventStreamReader(IEventStoreConnectionFactory eventStoreConnectionFactory)
        {
            _eventStoreConnectionFactory = eventStoreConnectionFactory;
        }

        public async Task<IEnumerable<(T EventData, EventMetadata EventMetadata)>> ReadEventsFromStream(string streamName)
        {
            var connection = await _eventStoreConnectionFactory.CreateConnectionAsync();

            var readEvents = await connection.ReadStreamEventsForwardAsync(
                streamName, StreamPosition.Start, 1000, true);

            var results = new List<(T EventData, EventMetadata EventMetadata)>();
            foreach (var @event in readEvents.Events)
            {
                var eventData = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));
                var eventMetadata = new EventMetadata
                {
                    Created = @event.Event.Created,
                    EventId = @event.Event.EventId,
                    EventNumber = @event.Event.EventNumber
                };
                results.Add((eventData, eventMetadata));
            }
            
            return results;
        }
    }
}