using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using events;
using events.Accounts;
using EventStore.Client;

namespace infrastructure.EventStore
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventStoreClientFactory _eventStoreClientFactory;

        public EventPublisher(IEventStoreClientFactory eventStoreClientFactory)
        {
            _eventStoreClientFactory = eventStoreClientFactory;
        }

        public async Task<bool> Publish<T>(T data, string streamName, CancellationToken cancellationToken)
        {
            _ = data ?? throw new ArgumentNullException(paramName: nameof(data));

            var metaData = data is IEvent @event ? new {Version = @event.Version()} : null;

            var eventData = new EventData(
                Uuid.NewUuid(),
                typeof(T).Name,
                JsonSerializer.SerializeToUtf8Bytes(data),
                JsonSerializer.SerializeToUtf8Bytes(metaData)
            );

            var client = _eventStoreClientFactory.CreateClient();

            await client.AppendToStreamAsync(
                streamName,
                StreamState.Any,
                new[] { eventData },
                cancellationToken: cancellationToken
            );

            return true;
        }
    }
}