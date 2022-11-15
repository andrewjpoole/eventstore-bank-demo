using Domain;
using Domain.Interfaces;
using OneOf;
using OneOf.Types;

namespace PaymentSchemeDomain.Events;

// Initial unvalidated payment event
public class InboundPaymentReceived_v1 : IEvent
{
    public Guid PaymentId { get; set; }
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

    public static PaymentDirection Direction => PaymentDirection.Inbound;
    public string StreamName() => PaymentSchemeDomainStreamNames.AccountPayments(Direction, DestinationSortCode, DestinationAccountNumber, PaymentId);
    public int Version() => 1;
}