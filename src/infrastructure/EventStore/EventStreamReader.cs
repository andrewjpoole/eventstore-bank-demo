using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
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

    public async Task<IEnumerable<IEventWrapper>> ReadForwards(string streamName, long startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true) =>
        await Read(streamName, Direction.Forwards, startPosition == -1 ? StreamPosition.Start : StreamPosition.FromInt64(startPosition), cancelationToken, maxCount, resolveLinkTos);

    public async Task<IEnumerable<IEventWrapper>> ReadBackwards(string streamName, long startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true) =>
        await Read(streamName, Direction.Backwards, startPosition == -1 ? StreamPosition.End : StreamPosition.FromInt64(startPosition), cancelationToken, maxCount, resolveLinkTos);

    private async Task<IEnumerable<IEventWrapper>> Read(string streamName, Direction direction, StreamPosition startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true)
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