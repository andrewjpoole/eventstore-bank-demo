using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using events;

namespace infrastructure.EventStore
{
    public class EventSubscription<T> : IEventSubscription<T> where T : IEvent
    {
        private readonly IEventStoreConnectionFactory _eventStoreConnectionFactory;

        public EventSubscription(IEventStoreConnectionFactory eventStoreConnectionFactory)
        {
            _eventStoreConnectionFactory = eventStoreConnectionFactory;
        }

        public async Task SubscribeToStream(string streamName, Action<T, EventMetadata> handleEventAppeared)
        {
            var connection = await _eventStoreConnectionFactory.CreateConnectionAsync();

            await connection.SubscribeToStreamAsync(streamName, true, (subscription, @event) =>
            {
                var eventData = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));
                handleEventAppeared(eventData, new EventMetadata
                {
                    Created = @event.Event.Created,
                    EventId = @event.Event.EventId,
                    EventNumber = @event.Event.EventNumber
                });
                return Task.CompletedTask;
            });
        }
    }
}