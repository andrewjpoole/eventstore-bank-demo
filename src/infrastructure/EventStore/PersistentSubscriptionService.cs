using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace infrastructure.EventStore
{
    public class PersistentSubscriptionService : IPersistentSubscriptionService
    {
        private readonly ILogger<PersistentSubscriptionService> _logger;
        private readonly IEventStoreClientFactory _eventStoreClientFactory;
        private PersistentSubscription _subscription;
        private EventStoreClient _client;
        private StreamPosition _checkpoint;
        private CancellationToken _cancellationToken;

        private Func<PersistentSubscription, ResolvedEvent, string, CancellationToken, Task> _handleEventAppeared;
        private string _streamName;
        private string _groupName;
        private string _subscriptionFriendlyName;
        private PersistentSubscriptionSettings _persistentSubscriptionSettings;
        private EventStorePersistentSubscriptionsClient _persistentSubscriptionsClient;

        public PersistentSubscriptionService(ILogger<PersistentSubscriptionService> logger, IEventStoreClientFactory eventStoreClientFactory)
        {
            _logger = logger;
            _eventStoreClientFactory = eventStoreClientFactory; // maynot need this one?

            var settings = EventStoreClientSettings.Create("");
            _persistentSubscriptionsClient = new EventStorePersistentSubscriptionsClient(settings);
        }

        public async Task StartAsync(
            string streamName,
            string groupName,
            string subscriptionFriendlyName,
            CancellationToken cancelationToken,
            PersistentSubscriptionSettings persistentSubscriptionSettings,
            Func<PersistentSubscription, ResolvedEvent, string, CancellationToken, Task> handleEventAppeared)
        {
            _streamName = string.IsNullOrEmpty(streamName) ? throw new ArgumentNullException(nameof(streamName)) : streamName;
            _groupName = string.IsNullOrEmpty(groupName) ? throw new ArgumentNullException(nameof(groupName)) : streamName;
            _subscriptionFriendlyName = string.IsNullOrEmpty(subscriptionFriendlyName) ? throw new ArgumentNullException(nameof(subscriptionFriendlyName)) : subscriptionFriendlyName;
            _persistentSubscriptionSettings = persistentSubscriptionSettings;
            _handleEventAppeared = handleEventAppeared ?? throw new ArgumentNullException(nameof(handleEventAppeared));

            _logger.LogInformation($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName}  is starting...");

            _cancellationToken = cancelationToken;
            _client = _eventStoreClientFactory.CreateClient();

            _checkpoint = StreamPosition.Start;
            await Subscribe();
        }

        private async Task Subscribe()
        {
            _logger.LogDebug($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} subscribing to {_streamName}...");
            
            _subscription = await _persistentSubscriptionsClient.SubscribeAsync(_streamName, _groupName, EventAppeared,
                SubscriptionDropped, autoAck: true, bufferSize: 10, cancellationToken: _cancellationToken);

            _logger.LogInformation($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} subscribed to {_streamName}");
        }

        private Task EventAppeared(PersistentSubscription subscription, ResolvedEvent @event, int? retryCount, CancellationToken cancellationToken)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(CatchupSubscription)}:{_subscriptionFriendlyName} Cancellation requested, stopping subscription...");
                subscription.Dispose();
                return Task.FromCanceled(_cancellationToken);
            }

            _checkpoint = @event.OriginalEventNumber;

            return _handleEventAppeared(subscription, @event, Encoding.UTF8.GetString(@event.Event.Data.ToArray()), _cancellationToken);
        }
        
        private void SubscriptionDropped(PersistentSubscription subscription, SubscriptionDroppedReason reason, Exception ex)
        {
            _logger.LogWarning($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} subscription dropped for reason: {reason} with exception {ex}");

            if (reason != SubscriptionDroppedReason.Disposed)
            {
                // Resubscribe if the client didn't stop the subscription
                _ = Subscribe();
            }
        }
    }
}