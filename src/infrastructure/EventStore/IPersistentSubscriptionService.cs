using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace infrastructure.EventStore
{
    public interface IPersistentSubscriptionService
    {
        Task StartAsync(
            string streamName,
            string groupName,
            string subscriptionFriendlyName,
            CancellationToken cancellationToken,
            Func<PersistentSubscription, ResolvedEvent, string, int?, CancellationToken, Task> handleEventAppeared);

        void Stop();
    }
}