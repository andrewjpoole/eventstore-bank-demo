using Domain;
using Domain.Interfaces;
using OneOf;
using OneOf.Types;
using PaymentSchemeDomain.Validation;

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
    
    public Guid CorrelationId { get; init; }

    public OneOf<True, List<string>> IsValid()
    {
        var errors = new List<string>();

        if(PaymentId == Guid.Empty)
            errors.Add("PaymentId must be a valid Guid");
        
        OriginatingSortCode.IsValidUKSortCode().UseError(s => errors.Add(s));
        DestinationSortCode.IsValidUKSortCode().UseError(s => errors.Add(s));
        OriginatingAccountNumber.IsValidUKAccountNumber().UseError(s => errors.Add(s));
        DestinationAccountNumber.IsValidUKAccountNumber().UseError(s => errors.Add(s));
        OriginatingAccountName.IsValidAccountName().UseError(s => errors.Add(s));
        DestinationAccountName.IsValidAccountName().UseError(s => errors.Add(s));
        PaymentReference.IsValidReference().UseError(s => errors.Add(s));

        return errors.Any() ? errors : new True();
    }

    public static PaymentDirection Direction => PaymentDirection.Inbound;
    public string StreamName() => PaymentSchemeDomainStreamNames.AccountPayments(Direction, DestinationSortCode, DestinationAccountNumber, PaymentId);
    public int Version() => 1;
}