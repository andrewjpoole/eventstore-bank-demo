using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using events;
using EventStore.ClientAPI;

namespace infrastructure.EventStore
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventStoreConnectionFactory _eventStoreConnectionFactory;

        public EventPublisher(IEventStoreConnectionFactory eventStoreConnectionFactory)
        {
            _eventStoreConnectionFactory = eventStoreConnectionFactory;
        }

        public async Task<bool> Publish<T>(T data, string streamName, CancellationToken cancellationToken)
        {
            _ = data ?? throw new ArgumentNullException(paramName: nameof(data));

            var metaData = data is IEvent @event ? new {Version = @event.Version()} : null;
            
            var connection = await _eventStoreConnectionFactory.CreateConnectionAsync();
            
            var eventPayload = new EventData(
                eventId: Guid.NewGuid(),
                type: typeof(T).Name,
                isJson: true,
                data: JsonSerializer.SerializeToUtf8Bytes(data),
                metadata: JsonSerializer.SerializeToUtf8Bytes(metaData)
            );
            var result = await connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventPayload);
            
            return true;
        }
    }
}