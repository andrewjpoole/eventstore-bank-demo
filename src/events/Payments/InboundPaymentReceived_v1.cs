using System;
using System.Text.Json.Serialization;
using events.Accounts;

namespace events.Payments
{
    // Initial unvalidated payment event
    public class InboundPaymentReceived_v1 : IEvent
    {
        public int OriginatingSortCode { get; init; }
        public int OriginatingAccountNumber { get; init; }
        public string OriginatingAccountName { get; set; }
        public string PaymentReference { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string DestinationAccountName { get; set; }
        public decimal Amount { get; init; }
        public PaymentScheme Scheme { get; init; }
        public PaymentType Type { get; init; }
        public DateTime ProcessingDate { get; init; }

        //public string StreamName() => Type switch
        //{
        //    "DirectCredit" => $"Account-{DestinationSortCode}-{DestinationAccountNumber}-Transactions",
        //    "DirectDebit" => $"Account-{}-{}-Transactions"
        //};

        public Guid CorrelationId { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentScheme
    {
        Bacs,
        Chaps,
        Fps
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
        Debit,
        Credit,
        RecalledCredit,
        DebitReversal,
        CreditReversal
    }

    public class InboundPaymentValidated_v1 : IEvent
    {
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }
    
    public class InboundPaymentSanctionsChecked_v1 : IEvent
    {
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }

    public class InboundPaymentAccountStatusChecked_v1 : IEvent
    {
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }

    public class InboundPaymentAccountBalanceChecked_v1 : IEvent
    {
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }

    public class InboundPaymentBalanceUpdated_v1 : IEvent
    {
        public Decimal Amount { get; set; }
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountBalanceLedger(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }

    public class InboundPaymentFailed_v1 : IEvent
    {
        public string Reason { get; init; }
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }

    public class InboundPaymentReturned_v1 : IEvent
    {
        public string Reason { get; init; }
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }

    public class InboundPaymentHeld_v1 : IEvent
    {
        public string Reason { get; init; }
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }

    public class InboundPaymentReleased_v1 : IEvent
    {
        public string Reason { get; init; }
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
    }
}
