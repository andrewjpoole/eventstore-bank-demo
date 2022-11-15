using Domain;
using Domain.Interfaces;
using OneOf;
using OneOf.Types;

namespace PaymentSchemeDomain.Events;

public class InboundHeldPaymentReleased_v1 : IEvent
{
    public Guid PaymentId { get; init; }
    public Guid CorrelationId { get; init; }
    public string ReleasedReason { get; init; }
    public string ReleasedBy { get; init; }
    public DateTime ReleasedAt { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public string StreamName() => PaymentSchemeDomainStreamNames.AccountPayments(PaymentDirection.Inbound, DestinationSortCode, DestinationAccountNumber, PaymentId);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}