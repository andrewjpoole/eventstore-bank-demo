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
    public class PaymentSanctionsCheckerHostedService : BackgroundService, IPaymentSanctionsCheckerHostedService
    {
        private readonly ILogger<PaymentSanctionsCheckerHostedService> _logger;
        private readonly IPersistentSubscriptionService _persistentSubscriptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ISanctionsApiClient _sanctionsApiClient;
        private readonly IInboundPaymentReadModelFactory _inboundPaymentReadModelFactory;
        private readonly IAccountDetailsReadModelFactory _accountDetailsReadModelFactory;

        private readonly string _streamName;
        private readonly string _subscriptionGroupName;
        private readonly string _subscriptionFriendlyName;


        public PaymentSanctionsCheckerHostedService(
            ILogger<PaymentSanctionsCheckerHostedService> logger, 
            IPersistentSubscriptionService persistentSubscriptionService, 
            IEventPublisher eventPublisher, 
            ISanctionsApiClient sanctionsApiClient, 
            IInboundPaymentReadModelFactory inboundPaymentReadModelFactory,
            IAccountDetailsReadModelFactory accountDetailsReadModelFactory)
        {
            _logger = logger;
            _persistentSubscriptionService = persistentSubscriptionService;
            _eventPublisher = eventPublisher;
            _sanctionsApiClient = sanctionsApiClient;
            _inboundPaymentReadModelFactory = inboundPaymentReadModelFactory;
            _accountDetailsReadModelFactory = accountDetailsReadModelFactory;

            _streamName = StreamNames.PaymentProcessing.AllInboundPaymentValidated;
            _subscriptionGroupName = StreamNames.SubscriptionGroupName(_streamName);
            _subscriptionFriendlyName = "Inbound-payment-validated";
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
                        nameof(InboundPaymentValidated_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentValidated_v1>(json), token),
                        _ => throw new NotImplementedException()
                    };
                });
        }

        public async Task HandleEvent(PersistentSubscription subscription, InboundPaymentValidated_v1 eventData, CancellationToken cancellationToken)
        {
            var paymentReadModel = await _inboundPaymentReadModelFactory.Create(eventData.DestinationSortCode, eventData.DestinationAccountNumber, eventData.CorrelationId, cancellationToken);
            var accountDetailsReadModel = await _accountDetailsReadModelFactory.Create(eventData.DestinationSortCode, eventData.DestinationAccountNumber, cancellationToken);

            // check all possible names
            var isSanctioned = await _sanctionsApiClient.CheckIfNameIsSanctioned(accountDetailsReadModel.Name);
            isSanctioned = await _sanctionsApiClient.CheckIfNameIsSanctioned(paymentReadModel.OriginatingAccountName);
            isSanctioned = await _sanctionsApiClient.CheckIfNameIsSanctioned(paymentReadModel.DestinationAccountName);

            // TODO some logging

            if (isSanctioned)
            {
                var nextEvent = new InboundPaymentHeld_v1()
                {
                    CorrelationId = eventData.CorrelationId,
                    DestinationSortCode = eventData.DestinationSortCode,
                    DestinationAccountNumber = eventData.DestinationAccountNumber
                };
                await _eventPublisher.Publish(nextEvent, nextEvent.StreamName(), CancellationToken.None);
            }
            else
            {
                var nextEvent = new InboundPaymentSanctionsChecked_v1()
                {
                    CorrelationId = eventData.CorrelationId,
                    DestinationSortCode = eventData.DestinationSortCode,
                    DestinationAccountNumber = eventData.DestinationAccountNumber
                };
                await _eventPublisher.Publish(nextEvent, nextEvent.StreamName(), CancellationToken.None);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _persistentSubscriptionService.Stop();
            return base.StopAsync(cancellationToken);
        }
    }

    public interface IAccountsReadModelFactory
    {
        Task<IAccountsReadModel> Create(int sortCode, int accountNumber);
    }

    public interface IAccountsReadModel
    {
        Task ReadAccountDetails(); // read all details events backwards until a snapshot, then read forwards populating state
        Task ReadTransactions(); // read all transactions events backwards until a snapshot, then read forwards populating state
        Task<decimal> ReadBalance(); // read all ledger events backwards until a snapshot, then read forwards populating state
    }
}