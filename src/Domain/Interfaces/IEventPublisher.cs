using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IEventPublisher
{
    Task<bool> Publish<T>(T data, string streamName, CancellationToken cancellationToken) where T: IEvent;

    Task<bool> Publish<T>(T data, string streamName, long expectedPosition, CancellationToken cancellationToken) where T : IEvent;
}