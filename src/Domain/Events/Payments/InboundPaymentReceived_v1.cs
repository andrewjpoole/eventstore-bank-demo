using System;
using System.Collections.Generic;
using OneOf;
using OneOf.Types;

namespace Domain.Events.Payments;

// Initial unvalidated payment event
public class InboundPaymentReceived_v1 : IEvent
{
    public string PaymentId { get; set; }
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

    public OneOf<True, List<string>> IsValid() => new True();

    public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber, CorrelationId);
    public int Version() => 1;
}