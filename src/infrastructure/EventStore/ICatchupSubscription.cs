using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace infrastructure.EventStore
{
    public interface ICatchupSubscription
    {
        Task StartAsync(
            string streamName, 
            string subscriptionFriendlyName, 
            CancellationToken cancelationToken, 
            CatchUpSubscriptionSettings catchupSubscriptionSettings, 
            Func<EventStoreCatchUpSubscription, ResolvedEvent, string, CancellationToken, Task> handleEventAppeared
            );
    }
}