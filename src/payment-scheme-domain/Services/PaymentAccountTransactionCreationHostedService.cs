using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using events;
using events.Payments;
using EventStore.Client;
using infrastructure.EventStore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace payment_scheme_domain.Services
{
    public class PaymentAccountTransactionCreationHostedService : BackgroundService, IPaymentAccountTransactionCreationHostedService
    {
        private readonly ILogger<PaymentAccountTransactionCreationHostedService> _logger;
        private readonly IPersistentSubscriptionService _persistentSubscriptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IInboundPaymentReadModelFactory _inboundPaymentReadModelFactory;

        private readonly string _streamName;
        private readonly string _subscriptionGroupName;
        private readonly string _subscriptionFriendlyName;

        public PaymentAccountTransactionCreationHostedService(ILogger<PaymentAccountTransactionCreationHostedService> logger, IPersistentSubscriptionService persistentSubscriptionService, IEventPublisher eventPublisher, IInboundPaymentReadModelFactory inboundPaymentReadModelFactory)
        {
            _logger = logger;
            _persistentSubscriptionService = persistentSubscriptionService;
            _eventPublisher = eventPublisher;
            _inboundPaymentReadModelFactory = inboundPaymentReadModelFactory;

            _streamName = StreamNames.PaymentProcessing.AllInboundPaymentAccountStatusChecked;
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
                (subscription, @event, json, retryCount, token) =>
                {
                    _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType} on {_subscriptionGroupName} retryCount: {retryCount}");
                    return @event.Event.EventType switch
                    {
                        nameof(InboundPaymentAccountStatusChecked_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentAccountStatusChecked_v1>(json), token),
                        _ => throw new NotImplementedException()
                    };
                });
        }

        public async Task HandleEvent(PersistentSubscription subscription, InboundPaymentAccountStatusChecked_v1 eventData, CancellationToken cancellationToken)
        {
            var paymentReadModel = await _inboundPaymentReadModelFactory.Create(eventData.DestinationSortCode, eventData.DestinationAccountNumber, eventData.CorrelationId);
            
            // simulate posting a transaction
            await Task.Delay(new Random().Next(200, 600));

            var nextEvent = new InboundPaymentBalanceUpdated_v1()
            {
                CorrelationId = eventData.CorrelationId,
                DestinationSortCode = eventData.DestinationSortCode,
                DestinationAccountNumber = eventData.DestinationAccountNumber,
                Amount = paymentReadModel.Amount
            };

            await _eventPublisher.Publish(nextEvent, nextEvent.StreamName(), CancellationToken.None);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _persistentSubscriptionService.Stop();
            return base.StopAsync(cancellationToken);
        }
    }
}