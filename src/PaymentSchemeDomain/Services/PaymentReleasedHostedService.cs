using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Events.Payments;
using Domain.Interfaces;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace payment_scheme_domain.Services;

public class PaymentReleasedHostedService : BackgroundService, IPaymentValidaterHostedService
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

        _streamName = StreamNames.Payments.AllInboundHeldPaymentReleased;
        _subscriptionGroupName = StreamNames.SubscriptionGroupName(_streamName);
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
                return HandleEvent(@event, token);
            });
    }

    public async Task HandleEvent(InboundPaymentReleased_v1 eventData, CancellationToken cancellationToken)
    {
        // TODO actually validate the payment data ???

        var nextEvent = new InboundPaymentSanctionsChecked_v1()
        {
            PaymentId = eventData.PaymentId,
            CorrelationId = eventData.CorrelationId,
            DestinationSortCode = eventData.DestinationSortCode,
            DestinationAccountNumber = eventData.DestinationAccountNumber
        };

        await _eventPublisher.Publish(nextEvent, nextEvent.StreamName(), cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _persistentSubscriptionService.Stop();
        return base.StopAsync(cancellationToken);
    }
}