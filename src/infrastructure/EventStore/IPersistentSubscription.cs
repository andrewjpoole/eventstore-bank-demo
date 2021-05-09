using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace infrastructure.EventStore
{
    public interface IPersistentSubscription
    {
        Task StartAsync(
            string streamName,
            string groupName,
            string subscriptionFriendlyName,
            CancellationToken cancelationToken,
            PersistentSubscriptionSettings persistentSubscriptionSettings,
            Func<EventStorePersistentSubscriptionBase, ResolvedEvent, string, CancellationToken, Task> handleEventAppeared);
    }
}