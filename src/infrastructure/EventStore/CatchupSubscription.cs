using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using EventStore.Client;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventStore;

public class CatchupSubscription : ICatchupSubscription, IDisposable
{
    private readonly ILogger<CatchupSubscription> _logger;
    private readonly IEventStoreClientFactory _eventStoreClientFactory;
    private StreamSubscription _subscription;
    private StreamPosition _checkpoint;
    private CancellationToken _cancellationToken;

    private Func<StreamSubscription, IEventWrapper, CancellationToken, Task> _handleEventAppeared;
    private string _streamName;
    private string _subscriptionFriendlyName;
    private EventStoreClient _client;

    public CatchupSubscription(ILogger<CatchupSubscription> logger, IEventStoreClientFactory eventStoreClientFactory)
    {
        _logger = logger;
        _eventStoreClientFactory = eventStoreClientFactory;
    }
        
    public async Task StartAsync(
        string streamName, 
        string subscriptionFriendlyName, 
        CancellationToken cancelationToken, 
        Func<StreamSubscription, IEventWrapper, CancellationToken, Task> handleEventAppeared)
    {
        _streamName = string.IsNullOrEmpty(streamName) ? throw new ArgumentNullException(nameof(_streamName)) : streamName;
        _subscriptionFriendlyName = string.IsNullOrEmpty(subscriptionFriendlyName) ? throw new ArgumentNullException(nameof(subscriptionFriendlyName)) : subscriptionFriendlyName;
        _handleEventAppeared = handleEventAppeared ?? throw new ArgumentNullException(nameof(handleEventAppeared));

        _logger.LogInformation($"{nameof(CatchupSubscription)}:{_subscriptionFriendlyName} is starting...");

        _cancellationToken = cancelationToken;
        _client = _eventStoreClientFactory.CreateClient();

        _checkpoint = StreamPosition.Start;
        await Subscribe();
    }
        
    private async Task Subscribe()
    {
        _logger.LogDebug($"{nameof(CatchupSubscription)}:{_subscriptionFriendlyName} subscribing to {_streamName}...");
            
        _subscription = await _client.SubscribeToStreamAsync(_streamName, FromStream.After(_checkpoint), EventAppeared, true, SubscriptionDropped, cancellationToken:_cancellationToken);
            
        _logger.LogInformation($"{nameof(CatchupSubscription)}:{_subscriptionFriendlyName} subscribed to {_streamName}");
    }

    private Task EventAppeared(StreamSubscription subscription, ResolvedEvent @event, CancellationToken cancellationToken)
    {
        if (_cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(CatchupSubscription)}:{_subscriptionFriendlyName} Cancellation requested, stopping subscription...");
            subscription.Dispose();
            return Task.FromCanceled(_cancellationToken);
        }

        _checkpoint = @event.OriginalEventNumber;

        var eventWrapper = new EventWrapper(@event);
        
        return _handleEventAppeared(subscription, eventWrapper, _cancellationToken);
    }
        
    private void SubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception ex)
    {
        _logger.LogWarning($"{nameof(CatchupSubscription)}:{_subscriptionFriendlyName} subscription dropped for reason: {reason} with exception {ex}");

        if (reason != SubscriptionDroppedReason.Disposed)
        {
            // Resubscribe if the client didn't stop the subscription
            _=Subscribe();
        }
    }

    public void Dispose()
    {
        _subscription?.Dispose();
        _client?.Dispose();
    }
}