using System;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface ICatchupSubscription : IDisposable
{
    Task StartAsync(
        string streamName,
        string subscriptionFriendlyName,
        CancellationToken cancelationToken,
        Func<IEventWrapper, CancellationToken, Task> handleEventAppeared);
}