using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IEventStreamReader
{
    Task<IEnumerable<IEventWrapper>> ReadBackwards(string streamName, long startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true);
    Task<IEnumerable<IEventWrapper>> ReadForwards(string streamName, long startPosition, CancellationToken cancelationToken, int maxCount = 1000, bool resolveLinkTos = true);
}

public static class StreamStartPositions
{
    public const long Default = -1;
}

