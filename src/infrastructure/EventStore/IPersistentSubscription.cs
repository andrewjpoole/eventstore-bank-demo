using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace infrastructure.EventStore
{
    public interface IPersistentSubscription
    {
        Task StartAsync(string streamName, string subscriptionFriendlyName, CancellationToken cancelationToken, Func<EventStorePersistentSubscription, ResolvedEvent, string, CancellationToken, Task> handleEventAppeared);
    }
}