using Domain;
using Domain.Interfaces;
using OneOf;
using OneOf.Types;

namespace PaymentSchemeDomain.Events;

public class InboundPaymentSanctionsChecked_v1 : IEvent
{
    public Guid PaymentId { get; init; }
    public Guid CorrelationId { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public static PaymentDirection Direction => PaymentDirection.Inbound;
    public string StreamName() => PaymentSchemeDomainStreamNames.AccountPayments(Direction, DestinationSortCode, DestinationAccountNumber, PaymentId);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}