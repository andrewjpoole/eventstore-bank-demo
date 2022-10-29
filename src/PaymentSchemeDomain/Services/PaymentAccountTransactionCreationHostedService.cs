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
using PaymentReadModel;

namespace payment_scheme_domain.Services;

public class PaymentAccountTransactionCreationHostedService : BackgroundService, IPaymentAccountTransactionCreationHostedService
{
    private readonly ILogger<PaymentAccountTransactionCreationHostedService> _logger;
    private readonly IPersistentSubscriptionService _persistentSubscriptionService;
    private readonly IEventDeserialiser _eventDeserialiser;
    private readonly IEventPublisher _eventPublisher;
    private readonly IInboundPaymentReadModelFactory _inboundPaymentReadModelFactory;

    private readonly string _streamName;
    private readonly string _subscriptionGroupName;
    private readonly string _subscriptionFriendlyName;

    public PaymentAccountTransactionCreationHostedService(ILogger<PaymentAccountTransactionCreationHostedService> logger, IPersistentSubscriptionService persistentSubscriptionService, IEventDeserialiser eventDeserialiser, IEventPublisher eventPublisher, IInboundPaymentReadModelFactory inboundPaymentReadModelFactory)
    {
        _logger = logger;
        _persistentSubscriptionService = persistentSubscriptionService;
        _eventDeserialiser = eventDeserialiser;
        _eventPublisher = eventPublisher;
        _inboundPaymentReadModelFactory = inboundPaymentReadModelFactory;

        _streamName = StreamNames.Payments.AllInboundPaymentAccountStatusChecked;
        _subscriptionGroupName = StreamNames.SubscriptionGroupName(_streamName);
        _subscriptionFriendlyName = "Inbound-payment-account-status-checked";
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

    public async Task HandleEvent(PersistentSubscription _, InboundPaymentAccountStatusChecked_v1 eventData, CancellationToken cancellationToken)
    {
        var paymentReadModel = await _inboundPaymentReadModelFactory.Create(InboundPaymentAccountStatusChecked_v1.Direction, eventData.DestinationSortCode, eventData.DestinationAccountNumber, eventData.PaymentId, cancellationToken);
            
        var transactionId = Guid.NewGuid(); // Todo replace this with a call to accounts to create the transaction etc

        var nextEvent = new InboundPaymentBalanceUpdated_v1()
        {
            PaymentId = eventData.PaymentId,
            CorrelationId = eventData.CorrelationId,
            DestinationSortCode = eventData.DestinationSortCode,
            DestinationAccountNumber = eventData.DestinationAccountNumber,
            Amount = paymentReadModel.Amount,
            ClearedTransactionId = transactionId
        };
            
        await _eventPublisher.Publish(nextEvent, nextEvent.StreamName(), CancellationToken.None);
            
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _persistentSubscriptionService.Stop();
        return base.StopAsync(cancellationToken);
    }
}