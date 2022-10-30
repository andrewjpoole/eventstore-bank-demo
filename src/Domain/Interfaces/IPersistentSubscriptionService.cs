using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IPersistentSubscriptionService
{
    Task StartAsync(
        string streamName,
        string groupName,
        string subscriptionFriendlyName,
        CancellationToken cancellationToken,
        Func<IEventWrapper, int?, CancellationToken, Task> handleEventAppeared);

    void Stop();
}