using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.EventStore.Serialisation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentSchemeDomain.Events;

namespace PaymentSchemeApp.Services;

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

        _streamName = PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllInboundPaymentReceived;
        _subscriptionGroupName = SharedStreamNames.SubscriptionGroupName(_streamName);
        _subscriptionFriendlyName = "Inbound-payment-received";
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

    public async Task HandleEvent(InboundPaymentReceived_v1 eventData, ulong eventNumber, CancellationToken cancellationToken)
    {
        var eventIsValid = eventData.IsValid();
        if (!eventIsValid.IsT0)
            throw new PermanentException($"Event failed validation. {string.Join(",", eventIsValid.AsT1)}");

        // ToDo Destination account must exist for an inbound payment to be valid
        // if exists, add the account name to the event ready for screening

        var nextEvent = new InboundPaymentValidated_v1
        {
            PaymentId = eventData.PaymentId,
            CorrelationId = eventData.CorrelationId,
            DestinationSortCode = eventData.DestinationSortCode,
            DestinationAccountNumber = eventData.DestinationAccountNumber
        };

        await _eventPublisher.Publish(nextEvent, nextEvent.StreamName(), eventNumber, CancellationToken.None);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _persistentSubscriptionService.Stop();
        return base.StopAsync(cancellationToken);
    }
}