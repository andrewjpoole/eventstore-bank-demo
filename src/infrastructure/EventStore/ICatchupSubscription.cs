using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace Infrastructure.EventStore;

public interface ICatchupSubscription : IDisposable
{
    Task StartAsync(
        string streamName,
        string subscriptionFriendlyName,
        CancellationToken cancelationToken,
        Func<StreamSubscription, ResolvedEvent, string, CancellationToken, Task> handleEventAppeared);
}