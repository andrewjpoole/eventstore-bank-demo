using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace Infrastructure.EventStore;

public interface IEventPublisher
{
    Task<bool> Publish<T>(T data, string streamName, CancellationToken cancellationToken);

    Task<bool> Publish<T>(T data, string streamName, StreamRevision streamRevision, CancellationToken cancellationToken);
}