using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using EventStore.Client;
using Infrastructure.EventStore.Serialisation;

namespace Infrastructure.EventStore;

public class EventStreamReader : IEventStreamReader
{
    private readonly IEventStoreClientFactory _eventStoreClientFactory;

    public EventStreamReader(IEventStoreClientFactory eventStoreClientFactory)
    {
        _eventStoreClientFactory = eventStoreClientFactory;
    }

    public async Task<IEnumerable<IEventWrapper>> Read(string streamName, Direction direction, StreamPosition startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true)
    {
        await using var client = _eventStoreClientFactory.CreateClient();

        var events = client.ReadStreamAsync(direction, streamName, startPosition, maxCount, resolveLinkTos:resolveLinkTos);

        var results = new List<EventWrapper>();
        await foreach (var @event in events)
        {
            if (cancelationToken.IsCancellationRequested)
                return results;
            
            var wrapper = new EventWrapper(@event);
            results.Add(wrapper);
        }
            
        return results;
    }
}