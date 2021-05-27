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
    public class InboundPaymentReadModel : IInboundPaymentReadModel
    {
        private readonly ILogger<InboundPaymentReadModel> _logger;
        private readonly IEventStreamReader _eventStreamReader;
        //private readonly ICatchupSubscription _catchupSubscription;
        private string _subscriptionFriendlyName;
        private CancellationTokenSource _cancellationTokenSource;

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


        public InboundPaymentReadModel(ILogger<InboundPaymentReadModel> logger, IEventStreamReader eventStreamReader) //ICatchupSubscription catchupSubscription)
        {
            _logger = logger;
            _eventStreamReader = eventStreamReader;
            //_catchupSubscription = catchupSubscription;
        }

        public async Task Read(int sortCode, int accountNumber, Guid correlationId, CancellationToken cancellationToken)
        {
            SortCode = sortCode;
            AccountNumber = accountNumber;
            CorrelationId = correlationId;

            _subscriptionFriendlyName = $"InboundPaymentReadModel-{SortCode}-{AccountNumber}";

            _cancellationTokenSource = new CancellationTokenSource();

            var events = await _eventStreamReader.Read(
                StreamNames.Accounts.AccountTransactions(SortCode, AccountNumber, correlationId), Direction.Forwards,
                StreamPosition.Start, cancellationToken);

            foreach (var @event in events)
            {
                _logger.LogDebug($"event read from stream #{@event.EventMetadata.EventNumber} {@event.typeName} on {_subscriptionFriendlyName}");
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                _ = @event.typeName switch
                {
                    nameof(InboundPaymentReceived_v1) => HandleEvent(JsonSerializer.Deserialize<InboundPaymentReceived_v1>(@event.json, options)),
                    nameof(InboundPaymentValidated_v1) => HandleEvent(JsonSerializer.Deserialize<InboundPaymentValidated_v1>(@event.json, options)),
                    nameof(InboundPaymentSanctionsChecked_v1) => HandleEvent(JsonSerializer.Deserialize<InboundPaymentSanctionsChecked_v1>(@event.json, options)),
                    nameof(InboundPaymentAccountStatusChecked_v1) => HandleEvent(JsonSerializer.Deserialize<InboundPaymentAccountStatusChecked_v1>(@event.json, options)),
                    nameof(InboundPaymentBalanceUpdated_v1) => HandleEvent(JsonSerializer.Deserialize<InboundPaymentBalanceUpdated_v1>(@event.json, options)),
                    _ => throw new NotImplementedException()
                };
            }

            //await _catchupSubscription.StartAsync(StreamNames.Accounts.AccountTransactions(SortCode, AccountNumber, correlationId),
            //    _subscriptionFriendlyName, _cancellationTokenSource.Token, (subscription, @event, json, cancellationToken) =>
            //    {
            //        _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType} on {_subscriptionFriendlyName}");
            //        var options = new JsonSerializerOptions
            //        {
            //            PropertyNameCaseInsensitive = true
            //        };
            //        return @event.Event.EventType switch
            //        {
            //            nameof(InboundPaymentReceived_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentReceived_v1>(json, options), cancellationToken),
            //            nameof(InboundPaymentValidated_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentValidated_v1>(json, options), cancellationToken),
            //            nameof(InboundPaymentSanctionsChecked_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentSanctionsChecked_v1>(json, options), cancellationToken),
            //            nameof(InboundPaymentAccountStatusChecked_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentAccountStatusChecked_v1>(json, options), cancellationToken),
            //            nameof(InboundPaymentBalanceUpdated_v1) => HandleEvent(subscription, JsonSerializer.Deserialize<InboundPaymentBalanceUpdated_v1>(json, options), cancellationToken),
            //            _ => throw new NotImplementedException()
            //        };
            //    });
        }

        private Task HandleEvent(InboundPaymentReceived_v1 eventData)
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

        private Task HandleEvent(InboundPaymentValidated_v1 eventData)
        {
            PaymentValidated = true;
            return Task.CompletedTask;
        }

        private Task HandleEvent(InboundPaymentSanctionsChecked_v1 eventData)
        {
            PassedSanctionsCheck = true;
            return Task.CompletedTask;
        }

        private Task HandleEvent(InboundPaymentAccountStatusChecked_v1 eventData)
        {
            PassedAccountStatusCheck = true;
            return Task.CompletedTask;
        }

        private Task HandleEvent(InboundPaymentBalanceUpdated_v1 eventData)
        {
            ClearedTransactionId = eventData.ClearedTransactionId;
            FundsCleared = true;
            return Task.CompletedTask;
        }
    }
}