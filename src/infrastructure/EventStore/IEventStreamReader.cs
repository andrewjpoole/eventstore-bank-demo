using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace Infrastructure.EventStore;

public interface IEventStreamReader
{
    Task<IEnumerable<(string typeName, string json, EventMetadata EventMetadata)>> Read(string streamName, Direction direction, StreamPosition startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true);
}