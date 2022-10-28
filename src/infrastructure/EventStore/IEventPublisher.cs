using System.Threading;
using System.Threading.Tasks;
using Domain;
using EventStore.Client;

namespace Infrastructure.EventStore;

public interface IEventPublisher
{
    Task<bool> Publish<T>(T data, string streamName, CancellationToken cancellationToken) where T: IEvent;

    Task<bool> Publish<T>(T data, string streamName, StreamRevision streamRevision, CancellationToken cancellationToken) where T : IEvent;
}