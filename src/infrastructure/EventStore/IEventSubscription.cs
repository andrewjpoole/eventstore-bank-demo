using System;
using System.Threading.Tasks;
using events;

namespace infrastructure.EventStore
{
    public interface IEventSubscription<T> where T : IEvent
    {
        Task SubscribeToStream(string streamName, Action<T, EventMetadata> handleEventAppeared);
    }
}