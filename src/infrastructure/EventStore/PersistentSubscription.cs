﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using events;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace infrastructure.EventStore
{
    public class PersistentSubscription : IPersistentSubscription
    {
        private readonly ILogger<PersistentSubscription> _logger;
        private readonly IEventStoreClientFactory _eventStoreClientFactory;
        private EventStorePersistentSubscriptionBase _subscription;
        private EventStoreClient _client;
        private StreamPosition _checkpoint;
        private CancellationToken _cancellationToken;

        private Func<EventStorePersistentSubscriptionBase, ResolvedEvent, string, CancellationToken, Task> _handleEventAppeared;
        private string _streamName;
        private string _groupName;
        private string _subscriptionFriendlyName;
        private PersistentSubscriptionSettings _persistentSubscriptionSettings;

        public PersistentSubscription(ILogger<PersistentSubscription> logger, IEventStoreClientFactory eventStoreClientFactory)
        {
            _logger = logger;
            _eventStoreClientFactory = eventStoreClientFactory;
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

            _logger.LogInformation($"{nameof(PersistentSubscription)}:{_subscriptionFriendlyName}  is starting...");

            _cancellationToken = cancelationToken;
            _client = _eventStoreClientFactory.CreateClient();

            _checkpoint = StreamPosition.Start;
            await Subscribe();
        }

        private async Task Subscribe()
        {
            _logger.LogDebug($"{nameof(PersistentSubscription)}:{_subscriptionFriendlyName} subscribing to {_streamName}...");

            _subscription = await _client. ConnectToPersistentSubscriptionAsync(_streamName, _groupName, EventAppeared, SubscriptionDropped);

            _logger.LogInformation($"{nameof(PersistentSubscription)}:{_subscriptionFriendlyName} subscribed to {_streamName}");
        }

        private Task EventAppeared(EventStorePersistentSubscriptionBase subscription, ResolvedEvent @event, int? arg3)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(PersistentSubscription)}:{_subscriptionFriendlyName} Cancellation requested, stopping subscription...");
                subscription.Stop(TimeSpan.FromSeconds(30));
                return Task.FromCanceled(_cancellationToken);
            }

            _checkpoint = @event.OriginalEventNumber;

            return _handleEventAppeared(subscription, @event, Encoding.UTF8.GetString(@event.Event.Data.ToArray()), _cancellationToken);
        }
        
        private void SubscriptionDropped(EventStorePersistentSubscriptionBase subscription, SubscriptionDroppedReason reason, Exception ex)
        {
            _logger.LogWarning($"{nameof(PersistentSubscription)}:{_subscriptionFriendlyName} subscription dropped for reason: {reason} with exception {ex}");

            if (reason != SubscriptionDroppedReason.Disposed)
            {
                // Resubscribe if the client didn't stop the subscription
                _ = Subscribe();
            }
        }
    }
}