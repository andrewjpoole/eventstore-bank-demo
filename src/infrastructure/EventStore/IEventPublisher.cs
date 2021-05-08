using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.EventStore
{
    public interface IEventPublisher
    {
        Task<bool> Publish<T>(T data, string streamName, CancellationToken cancellationToken);
    }
}