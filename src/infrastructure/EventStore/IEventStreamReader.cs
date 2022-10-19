using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using EventStore.Client;

namespace Infrastructure.EventStore;

public interface IEventStreamReader
{
    Task<IEnumerable<IEventWrapper>> Read(string streamName, Direction direction, StreamPosition startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true);
}