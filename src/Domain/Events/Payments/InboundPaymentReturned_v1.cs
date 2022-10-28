using System;
using System.Collections.Generic;
using OneOf;
using OneOf.Types;

namespace Domain.Events.Payments;

public class InboundPaymentReturned_v1 : IEvent
{
    public Guid PaymentId { get; init; }
    public Guid CorrelationId { get; init; }
    public string Reason { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public static PaymentDirection Direction => PaymentDirection.Inbound;
    public string StreamName() => StreamNames.Payments.AccountPayments(Direction, DestinationSortCode, DestinationAccountNumber, PaymentId);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}