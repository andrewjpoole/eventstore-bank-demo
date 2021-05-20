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
    public interface IPaymentReceivedHostedService
    {
    }

    public class PaymentReceivedHostedService : BackgroundService, IPaymentReceivedHostedService
    {
        private readonly ILogger<PaymentReceivedHostedService> _logger;
        private readonly IPersistentSubscriptionService _persistentSubscriptionService;
        private readonly IEventPublisher _eventPublisher;

        private readonly string _subscriptionGroupName = StreamNames.SubscriptionGroupName(StreamNames.PaymentProcessing.InboundPaymentReceived);

        public PaymentReceivedHostedService(ILogger<PaymentReceivedHostedService> logger, IPersistentSubscriptionService persistentSubscriptionService, IEventPublisher eventPublisher)
        {
            _logger = logger;
            _persistentSubscriptionService = persistentSubscriptionService;
            _eventPublisher = eventPublisher;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return _persistentSubscriptionService.StartAsync(
                StreamNames.PaymentProcessing.InboundPaymentReceived,
                _subscriptionGroupName,
                "Inbound-payment-received",
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
}
