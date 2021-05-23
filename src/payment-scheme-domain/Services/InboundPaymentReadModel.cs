using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using events;
using events.Payments;
using EventStore.Client;
using infrastructure.EventStore;
using Microsoft.Extensions.Logging;

namespace payment_scheme_domain.Services
{
    public class InboundPaymentReadModel
    {
        private readonly ILogger<InboundPaymentReadModel> _logger;
        private readonly ICatchupSubscription _catchupSubscription;
        private string _subscriptionFriendlyName;

        public int SortCode { get; private set; }
        public int AccountNumber { get; private set; }
        public Guid CorrelationId { get; private set; }

        public int OriginatingSortCode { get; private set; }
        public int OriginatingAccountNumber { get; private set; }
        public string OriginatingAccountName { get; private set; }
        public string PaymentReference { get; private set; }
        public string DestinationAccountName { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentScheme Scheme { get; private set; }
        public PaymentType Type { get; private set; }
        public DateTime ProcessingDate { get; private set; }

        public bool PaymentValidated { get; private set; }
        public bool PassedSanctionsCheck { get; private set; }
        public bool PassedAccountStatusCheck { get; private set; }
        public bool FundsCleared { get; private set; }
        public Guid ClearedTransactionId { get; private set; }


        public InboundPaymentReadModel(ILogger<InboundPaymentReadModel> logger, ICatchupSubscription catchupSubscription)
        {
            _logger = logger;
            _catchupSubscription = catchupSubscription;
        }

        public async Task Read(int sortCode, int accountNumber, Guid correlationId)
        {
            SortCode = sortCode;
            AccountNumber = accountNumber;
            CorrelationId = correlationId;

            _subscriptionFriendlyName = $"InboundPaymentReadModel-{SortCode}-{AccountNumber}";
            
            await _catchupSubscription.StartAsync(StreamNames.Accounts.AccountTransactions(SortCode, AccountNumber, correlationId),
                _subscriptionFriendlyName, CancellationToken.None, (subscription, @event, json, cancellationToken) =>
                {
                    _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType} on {_subscriptionFriendlyName}");
                    return @event.Event.EventType switch
                    {
                        nameof(InboundPaymentReceived_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentReceived_v1>(json), cancellationToken),
                        nameof(InboundPaymentValidated_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentValidated_v1>(json), cancellationToken),
                        nameof(InboundPaymentSanctionsChecked_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentSanctionsChecked_v1>(json), cancellationToken),
                        nameof(InboundPaymentAccountStatusChecked_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentAccountStatusChecked_v1>(json), cancellationToken),
                        nameof(InboundPaymentBalanceUpdated_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentBalanceUpdated_v1>(json), cancellationToken),
                        _ => throw new NotImplementedException()
                    };
                });
        }

        private Task HandleEvent(StreamSubscription subscription, InboundPaymentReceived_v1 eventData, CancellationToken cancellationToken)
        {
            OriginatingSortCode = eventData.OriginatingSortCode;
            OriginatingAccountNumber = eventData.OriginatingAccountNumber;
            OriginatingAccountName = eventData.OriginatingAccountName;
            DestinationAccountName = eventData.DestinationAccountName;
            PaymentReference = eventData.PaymentReference;
            Amount = eventData.Amount;
            Scheme = eventData.Scheme;
            Type = eventData.Type;
            ProcessingDate = eventData.ProcessingDate;

            return Task.CompletedTask;
        }

        private Task HandleEvent(StreamSubscription subscription, InboundPaymentValidated_v1 eventData, CancellationToken cancellationToken)
        {
            PaymentValidated = true;
            return Task.CompletedTask;
        }

        private Task HandleEvent(StreamSubscription subscription, InboundPaymentSanctionsChecked_v1 eventData, CancellationToken cancellationToken)
        {
            PassedSanctionsCheck = true;
            return Task.CompletedTask;
        }

        private Task HandleEvent(StreamSubscription subscription, InboundPaymentAccountStatusChecked_v1 eventData, CancellationToken cancellationToken)
        {
            PassedAccountStatusCheck = true;
            return Task.CompletedTask;
        }

        private Task HandleEvent(StreamSubscription subscription, InboundPaymentBalanceUpdated_v1 eventData, CancellationToken cancellationToken)
        {
            ClearedTransactionId = eventData.ClearedTransactionId;
            FundsCleared = true;
            return Task.CompletedTask;
        }
    }
}