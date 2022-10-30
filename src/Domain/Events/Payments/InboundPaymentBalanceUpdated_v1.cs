using System;
using System.Collections.Generic;
using Domain.Interfaces;
using OneOf;
using OneOf.Types;

namespace Domain.Events.Payments;

public class InboundPaymentBalanceUpdated_v1 : IEvent
{
    public Guid PaymentId { get; init; }
    public Guid CorrelationId { get; init; }
    public Decimal Amount { get; set; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public Guid ClearedTransactionId { get; set; }
    public static PaymentDirection Direction => PaymentDirection.Inbound;
    public string StreamName() => StreamNames.Payments.AccountPayments(Direction, DestinationSortCode, DestinationAccountNumber, PaymentId);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}