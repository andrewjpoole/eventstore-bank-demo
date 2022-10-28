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

public class PaymentAccountStatusCheckerHostedService : BackgroundService, IPaymentAccountStatusCheckerHostedService
{
    private readonly ILogger<PaymentValidaterHostedService> _logger;
    private readonly IPersistentSubscriptionService _persistentSubscriptionService;
    private readonly IEventPublisher _eventPublisher;
    private readonly IEventDeserialiser _eventDeserialiser;

    private readonly string _streamName;
    private readonly string _subscriptionGroupName;
    private readonly string _subscriptionFriendlyName;

    public PaymentAccountStatusCheckerHostedService(ILogger<PaymentValidaterHostedService> logger, IPersistentSubscriptionService persistentSubscriptionService, IEventPublisher eventPublisher, IEventDeserialiser eventDeserialiser)
    {
        _logger = logger;
        _persistentSubscriptionService = persistentSubscriptionService;
        _eventPublisher = eventPublisher;
        _eventDeserialiser = eventDeserialiser;

        _streamName = StreamNames.PaymentProcessing.AllInboundPaymentSanctionsChecked;
        _subscriptionGroupName = StreamNames.SubscriptionGroupName(_streamName);
        _subscriptionFriendlyName = "Inbound-payment-sanctions-checked";
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

    public async Task HandleEvent(PersistentSubscription subscription, InboundPaymentSanctionsChecked_v1 eventData, CancellationToken cancellationToken)
    {
        // simulate checking account status
        await Task.Delay(new Random().Next(200, 600));
            
        var nextEvent = new InboundPaymentAccountStatusChecked_v1()
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