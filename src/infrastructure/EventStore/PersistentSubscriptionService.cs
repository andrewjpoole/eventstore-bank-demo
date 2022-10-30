using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using EventStore.Client;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventStore;

public class PersistentSubscriptionService : IPersistentSubscriptionService
{
    private readonly ILogger<PersistentSubscriptionService> _logger;
    private PersistentSubscription _subscription;
    private CancellationToken _cancellationToken;

    private Func<IEventWrapper, int?, CancellationToken, Task> _handleEventAppeared;
    private string _streamName;
    private string _groupName;
    private string _subscriptionFriendlyName;
    private readonly EventStorePersistentSubscriptionsClient _persistentSubscriptionsClient;

    // TODO get some stats in here

    public PersistentSubscriptionService(ILogger<PersistentSubscriptionService> logger)
    {
        _logger = logger;

        var credentials = new UserCredentials("admin", "changeit");
        var settings = new EventStoreClientSettings
        {
            CreateHttpMessageHandler = () =>
                new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, certificate2, x509Chain, sslPolicyErrors) => true // ignore https
                },
            ConnectivitySettings =
            {
                Address = new Uri("http://localhost:2113"),
                Insecure = true
            },
            DefaultCredentials = credentials,
            ConnectionName = "ajp"
        };

        _persistentSubscriptionsClient = new EventStorePersistentSubscriptionsClient(settings);
            
    }

    public async Task StartAsync(
        string streamName,
        string groupName,
        string subscriptionFriendlyName,
        CancellationToken cancellationToken,
        Func<IEventWrapper, int?, CancellationToken, Task> handleEventAppeared)
    {
        _streamName = string.IsNullOrEmpty(streamName) ? throw new ArgumentNullException(nameof(streamName)) : streamName;
        _groupName = string.IsNullOrEmpty(groupName) ? throw new ArgumentNullException(nameof(groupName)) : groupName;
        _subscriptionFriendlyName = string.IsNullOrEmpty(subscriptionFriendlyName) ? throw new ArgumentNullException(nameof(subscriptionFriendlyName)) : subscriptionFriendlyName;
        _handleEventAppeared = handleEventAppeared ?? throw new ArgumentNullException(nameof(handleEventAppeared));

        _logger.LogInformation($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName}  is starting...");

        _cancellationToken = cancellationToken;
        await Subscribe();
    }

    public void Stop()
    {
        _logger.LogInformation($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} StopAsync called, disposing subscription...");
        _subscription.Dispose();
    }

    private async Task Subscribe()
    {
        _logger.LogDebug($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} subscribing to {_streamName}...");

        try
        {
            _subscription = await _persistentSubscriptionsClient.SubscribeToStreamAsync(_streamName, _groupName, EventAppeared,
                SubscriptionDropped, bufferSize: 10, cancellationToken: _cancellationToken);

            _logger.LogInformation($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} subscribed to {_streamName}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} exception subscribing to {_streamName} {e}");
        }
    }

    private Task EventAppeared(PersistentSubscription subscription, ResolvedEvent @event, int? retryCount, CancellationToken cancellationToken)
    {
        if (_cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(CatchupSubscription)}:{_subscriptionFriendlyName} Cancellation requested, stopping subscription...");
            subscription.Dispose();
            return Task.FromCanceled(_cancellationToken);
        }

        try
        {
            var eventWrapper = new EventWrapper(@event);
            _handleEventAppeared(eventWrapper, retryCount, _cancellationToken).GetAwaiter().GetResult();
            subscription.Ack(@event);
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception while handling event from {_subscriptionFriendlyName}: {e}");
            throw;
        }
    }
        
    private void SubscriptionDropped(PersistentSubscription subscription, SubscriptionDroppedReason reason, Exception ex)
    {
        _logger.LogWarning($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} subscription dropped for reason: {reason} with exception {ex}");

        if (reason != SubscriptionDroppedReason.Disposed)
        {
            // Resubscribe if the client didn't stop the subscription
            Task.Delay(TimeSpan.FromSeconds(2)).GetAwaiter().GetResult();
            _logger.LogInformation($"{nameof(PersistentSubscriptionService)}:{_subscriptionFriendlyName} attempting to resubscribe...");
            _ = Subscribe();
        }
    }
}