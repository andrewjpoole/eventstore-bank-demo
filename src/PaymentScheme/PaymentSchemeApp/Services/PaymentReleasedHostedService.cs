﻿using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Interfaces;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentSchemeDomain.Events;

namespace PaymentSchemeApp.Services;

public class PaymentReleasedHostedService : BackgroundService, IPaymentReleasedHostedService
{
    private readonly ILogger<PaymentReleasedHostedService> _logger;
    private readonly IPersistentSubscriptionService _persistentSubscriptionService;
    private readonly IEventDeserialiser _eventDeserialiser;
    private readonly IEventPublisher _eventPublisher;

    private readonly string _streamName;
    private readonly string _subscriptionGroupName;
    private readonly string _subscriptionFriendlyName;

    public PaymentReleasedHostedService(ILogger<PaymentReleasedHostedService> logger, IPersistentSubscriptionService persistentSubscriptionService, IEventDeserialiser eventDeserialiser, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _persistentSubscriptionService = persistentSubscriptionService;
        _eventDeserialiser = eventDeserialiser;
        _eventPublisher = eventPublisher;

        _streamName = PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllInboundHeldPaymentReleased;
        _subscriptionGroupName = SharedStreamNames.SubscriptionGroupName(_streamName);
        _subscriptionFriendlyName = "Inbound-payment-released";
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _persistentSubscriptionService.StartAsync(
            _streamName,
            _subscriptionGroupName,
            _subscriptionFriendlyName,
            cancellationToken,
            (eventWrapper, retryCount, token) =>
            {
                _logger.LogTrace($"event appeared #{eventWrapper.EventNumber} {eventWrapper.EventTypeName} on {_subscriptionGroupName} retryCount: {retryCount}");
                dynamic @event = _eventDeserialiser.DeserialiseEvent(eventWrapper);
                return HandleEvent(@event, eventWrapper.EventNumber, token);
            });
    }

    public async Task HandleEvent(InboundHeldPaymentReleased_v1 eventData, ulong eventNumber, CancellationToken cancellationToken)
    {
        var nextEvent = new InboundPaymentSanctionsChecked_v1()
        {
            PaymentId = eventData.PaymentId,
            CorrelationId = eventData.CorrelationId,
            DestinationSortCode = eventData.DestinationSortCode,
            DestinationAccountNumber = eventData.DestinationAccountNumber
        };

        await _eventPublisher.Publish(nextEvent, nextEvent.StreamName(), eventNumber, cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _persistentSubscriptionService.Stop();
        return base.StopAsync(cancellationToken);
    }
}