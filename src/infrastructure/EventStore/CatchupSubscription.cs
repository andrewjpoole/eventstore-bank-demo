using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;

namespace infrastructure.EventStore
{
    public class CatchupSubscription : ICatchupSubscription
    {
        private readonly ILogger<CatchupSubscription> _logger;
        private readonly IEventStoreConnectionFactory _eventStoreConnectionFactory;
        private EventStoreCatchUpSubscription _subscription;
        private IEventStoreConnection _connection;
        private long _checkpoint;
        private CancellationToken _cancellationToken;

        private Func<EventStoreCatchUpSubscription, ResolvedEvent, string, CancellationToken, Task> _handleEventAppeared;
        private string _streamName;
        private string _subscriptionFriendlyName;
        private CatchUpSubscriptionSettings _catchupSubscriptionSettings;

        public CatchupSubscription(ILogger<CatchupSubscription> logger, IEventStoreConnectionFactory eventStoreConnectionFactory)
        {
            _logger = logger;
            _eventStoreConnectionFactory = eventStoreConnectionFactory;
        }
        
        public async Task StartAsync(
            string streamName, 
            string subscriptionFriendlyName, 
            CancellationToken cancelationToken, 
            CatchUpSubscriptionSettings catchupSubscriptionSettings, 
            Func<EventStoreCatchUpSubscription, ResolvedEvent, string, CancellationToken, Task> handleEventAppeared)
        {
            _streamName = string.IsNullOrEmpty(streamName) ? throw new ArgumentNullException(nameof(_streamName)) : streamName;
            _subscriptionFriendlyName = string.IsNullOrEmpty(subscriptionFriendlyName) ? throw new ArgumentNullException(nameof(subscriptionFriendlyName)) : subscriptionFriendlyName;
            _catchupSubscriptionSettings = catchupSubscriptionSettings;
            _handleEventAppeared = handleEventAppeared ?? throw new ArgumentNullException(nameof(handleEventAppeared));

            _logger.LogInformation($"CatchupSubscription named {_subscriptionFriendlyName} is starting...");

            _cancellationToken = cancelationToken;
            _connection = await _eventStoreConnectionFactory.CreateConnectionAsync();

            _checkpoint = StreamPosition.Start;
            Subscribe();
        }
        
        private void Subscribe()
        {
            _logger.LogInformation($"Subscribing to {_streamName} for {_subscriptionFriendlyName}...");

            _subscription = _connection.SubscribeToStreamFrom (_streamName, _checkpoint, _catchupSubscriptionSettings, EventAppeared, LiveProcessingStarted, SubscriptionDropped);

            _logger.LogInformation($"Subscribed to {_streamName} for {_subscriptionFriendlyName}");
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription obj)
        {
            _logger.LogInformation($"{_subscriptionFriendlyName} has caught up and is now processing new events as they arrive");
        }

        private Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent @event)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Cancellation requested for {_subscriptionFriendlyName}, disposing of subscription...");
                subscription.Stop();
                return Task.FromCanceled(_cancellationToken);
            }

            _checkpoint = @event.OriginalEventNumber;

            return _handleEventAppeared(subscription, @event, Encoding.UTF8.GetString(@event.Event.Data.ToArray()), _cancellationToken);
        }

        private void SubscriptionDropped(EventStoreCatchUpSubscription subsctipion, SubscriptionDropReason reason, Exception ex)
        {
            _logger.LogWarning($"Subscription named {_subscriptionFriendlyName} to {SubscriptionNames.Sanctions.GlobalSanctionedNames} dropped for reason: {reason} with exception {ex}");

            if (reason != SubscriptionDropReason.ConnectionClosed)
            {
                // Resubscribe if the client didn't stop the subscription
                Subscribe();
            }
        }
    }
}