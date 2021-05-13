using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using events;

namespace infrastructure.EventStore
{
    public class EventSubscription<T> : IEventSubscription<T> where T : IEvent
    {
        private readonly IEventStoreClientFactory _eventStoreClientFactory;

        public EventSubscription(IEventStoreClientFactory eventStoreClientFactory)
        {
            _eventStoreClientFactory = eventStoreClientFactory;
        }

        public async Task SubscribeToStream(string streamName, Action<T, EventMetadata> handleEventAppeared)
        {
            var client = _eventStoreClientFactory.CreateClient();

            await client.SubscribeToStreamAsync(streamName, (subscription, @event, cancellationToken) =>
            {
                var eventData = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));
                handleEventAppeared(eventData, new EventMetadata
                {
                    Created = @event.Event.Created,
                    EventId = @event.Event.EventId.ToGuid(),
                    EventNumber = @event.Event.EventNumber
                });
                return Task.CompletedTask;
            });
        }
    }
}