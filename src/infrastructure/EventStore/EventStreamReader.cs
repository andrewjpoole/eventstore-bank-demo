using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using events;
using EventStore.Client;

namespace infrastructure.EventStore
{
    public class EventStreamReader<T> : IEventStreamReader<T> where T : IEvent
    {
        private readonly IEventStoreClientFactory _eventStoreClientFactory;

        public EventStreamReader(IEventStoreClientFactory eventStoreClientFactory)
        {
            _eventStoreClientFactory = eventStoreClientFactory;
        }

        public async Task<IEnumerable<(T EventData, EventMetadata EventMetadata)>> ReadEventsFromStream(string streamName)
        {
            var client = _eventStoreClientFactory.CreateClient();

            var events = client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start, 1000, resolveLinkTos:true);

            var results = new List<(T EventData, EventMetadata EventMetadata)>();
            await foreach (var @event in events)
            {
                var eventData = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));
                var eventMetadata = new EventMetadata
                {
                    Created = @event.Event.Created,
                    EventId = @event.Event.EventId.ToGuid(),
                    EventNumber = @event.Event.EventNumber
                };
                results.Add((eventData, eventMetadata));
            }
            
            return results;
        }
    }
}