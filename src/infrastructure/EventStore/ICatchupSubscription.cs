using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using EventStore.Client;

namespace Infrastructure.EventStore;

public interface ICatchupSubscription : IDisposable
{
    Task StartAsync(
        string streamName,
        string subscriptionFriendlyName,
        CancellationToken cancelationToken,
        Func<StreamSubscription, IEventWrapper, CancellationToken, Task> handleEventAppeared);
}