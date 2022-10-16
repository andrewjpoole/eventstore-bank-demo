using System;
using System.Threading.Tasks;
using Domain;

namespace Infrastructure.EventStore;

public interface IEventSubscription<T> where T : IEvent
{
    Task SubscribeToStream(string streamName, Action<T, EventMetadata> handleEventAppeared);
}