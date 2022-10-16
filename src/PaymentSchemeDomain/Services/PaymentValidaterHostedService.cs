using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Events.Payments;
using EventStore.Client;
using Infrastructure.EventStore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace payment_scheme_domain.Services;

public class PaymentValidaterHostedService : BackgroundService, IPaymentValidaterHostedService
{
    private readonly ILogger<PaymentValidaterHostedService> _logger;
    private readonly IPersistentSubscriptionService _persistentSubscriptionService;
    private readonly IEventPublisher _eventPublisher;

    private readonly string _streamName;
    private readonly string _subscriptionGroupName;
    private readonly string _subscriptionFriendlyName;

    public PaymentValidaterHostedService(ILogger<PaymentValidaterHostedService> logger, IPersistentSubscriptionService persistentSubscriptionService, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _persistentSubscriptionService = persistentSubscriptionService;
        _eventPublisher = eventPublisher;

        _streamName = StreamNames.PaymentProcessing.AllInboundPaymentReceived;
        _subscriptionGroupName = StreamNames.SubscriptionGroupName(_streamName);
        _subscriptionFriendlyName = "Inbound-payment-received";
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _persistentSubscriptionService.StartAsync(
            _streamName,
            _subscriptionGroupName,
            _subscriptionFriendlyName,
            cancellationToken,
            (subscription, @event, json, retryCount, token) => 
            {
                _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType} on {_subscriptionGroupName} retryCount: {retryCount}");
                return @event.Event.EventType switch
                {
                    nameof(InboundPaymentReceived_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentReceived_v1>(json), token),
                    _ => throw new NotImplementedException()
                };
            });
    }

    public async Task HandleEvent(PersistentSubscription subscription, InboundPaymentReceived_v1 eventData, CancellationToken cancellationToken)
    {
        // simulate some work and publish the next event...
        await Task.Delay(new Random().Next(200, 600));

        // TODO actually validate the payment data ???

        var nextEvent = new InboundPaymentValidated_v1
        {
            CorrelationId = eventData.CorrelationId,
            DestinationSortCode = eventData.DestinationSortCode,
            DestinationAccountNumber = eventData.DestinationAccountNumber
        };

        await _eventPublisher.Publish(nextEvent, nextEvent.StreamName(), CancellationToken.None);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _persistentSubscriptionService.Stop();
        return base.StopAsync(cancellationToken);
    }
}