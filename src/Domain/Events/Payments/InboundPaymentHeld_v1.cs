﻿using System;
using System.Collections.Generic;
using OneOf;
using OneOf.Types;

namespace Domain.Events.Payments;

public class InboundPaymentHeld_v1 : IEvent
{
    public string Reason { get; init; }
    public string PaymentId { get; init; }
    public Guid CorrelationId { get; init; }
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
    public string StreamName() => StreamNames.Accounts.AccountTransactionsSanctions(DestinationSortCode, DestinationAccountNumber, CorrelationId);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}

public class HeldPayment
{
    public string Reason { get; init; }
    public string PaymentId { get; init; }
    public Guid CorrelationId { get; init; }
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

    public Guid HeldPaymentReleaseToken { get; set; }

    public bool IsReleased { get; private set; }
    public DateTime ReleasedAt { get; private set; }
    public string ReleasedBy { get; private set; }
    public string ReleasedReason { get; private set; }

    public void Release(InboundHeldPaymentReleased_v1 @event)
    {
        IsReleased = true;
        ReleasedAt = @event.ReleasedAt;
        ReleasedBy = @event.ReleasedBy;
        ReleasedReason = @event.ReleasedReason;
    }
}

public class InboundHeldPaymentReleased_v1 : IEvent
{
    public string PaymentId { get; init; }
    public string ReleasedReason { get; init; }
    public string ReleasedBy { get; init; }
    public DateTime ReleasedAt { get; init; }
    public Guid CorrelationId { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public string StreamName() => StreamNames.Accounts.AccountTransactionsSanctions(DestinationSortCode, DestinationAccountNumber, CorrelationId);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}