using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace Infrastructure.EventStore;

public class EventStreamReader : IEventStreamReader
{
    private readonly IEventStoreClientFactory _eventStoreClientFactory;

    public EventStreamReader(IEventStoreClientFactory eventStoreClientFactory)
    {
        _eventStoreClientFactory = eventStoreClientFactory;
    }

    public async Task<IEnumerable<(string typeName, string json, EventMetadata EventMetadata)>> Read(string streamName, Direction direction, StreamPosition startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true)
    {
        await using var client = _eventStoreClientFactory.CreateClient();

        var events = client.ReadStreamAsync(direction, streamName, startPosition, maxCount, resolveLinkTos:resolveLinkTos);

        var results = new List<(string typeName, string json, EventMetadata EventMetadata)>();
        await foreach (var @event in events)
        {
            if (cancelationToken.IsCancellationRequested)
                return results;

            var eventMetadata = new EventMetadata
            {
                Created = @event.OriginalEvent.Created,
                EventId = @event.OriginalEvent.EventId.ToGuid(),
                EventNumber = @event.OriginalEvent.EventNumber
            };
            results.Add((@event.OriginalEvent.EventType, Encoding.UTF8.GetString(@event.OriginalEvent.Data.ToArray()), eventMetadata));
        }
            
        return results;
    }
}