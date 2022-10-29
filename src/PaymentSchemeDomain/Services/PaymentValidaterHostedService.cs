using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Events.Payments;
using EventStore.Client;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace payment_scheme_domain.Services;

public class PaymentValidaterHostedService : BackgroundService, IPaymentValidaterHostedService
{
    private readonly ILogger<PaymentValidaterHostedService> _logger;
    private readonly IPersistentSubscriptionService _persistentSubscriptionService;
    private readonly IEventDeserialiser _eventDeserialiser;
    private readonly IEventPublisher _eventPublisher;

    private readonly string _streamName;
    private readonly string _subscriptionGroupName;
    private readonly string _subscriptionFriendlyName;

    public PaymentValidaterHostedService(ILogger<PaymentValidaterHostedService> logger, IPersistentSubscriptionService persistentSubscriptionService, IEventDeserialiser eventDeserialiser, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _persistentSubscriptionService = persistentSubscriptionService;
        _eventDeserialiser = eventDeserialiser;
        _eventPublisher = eventPublisher;

        _streamName = StreamNames.Payments.AllInboundPaymentReceived;
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
            (subscription, eventWrapper, retryCount, token) =>
            {
                _logger.LogTrace($"event appeared #{eventWrapper.EventNumber} {eventWrapper.EventTypeName} on {_subscriptionGroupName} retryCount: {retryCount}");
                dynamic @event = _eventDeserialiser.DeserialiseEvent(eventWrapper);
                return HandleEvent(subscription, @event, token);
            });
    }

    public async Task HandleEvent(PersistentSubscription _, InboundPaymentReceived_v1 eventData, CancellationToken cancellationToken)
    {
        // simulate some work and publish the next event...
        await Task.Delay(new Random().Next(200, 600), cancellationToken);

        // TODO actually validate the payment data ???

        var nextEvent = new InboundPaymentValidated_v1
        {
            PaymentId = eventData.PaymentId,
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