using System;
using System.Collections.Generic;
using OneOf;
using OneOf.Types;

namespace Domain.Events.Payments;

public class InboundPaymentBalanceUpdated_v1 : IEvent
{
    public Decimal Amount { get; set; }
    public Guid CorrelationId { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public Guid ClearedTransactionId { get; set; }

    public string StreamName() => StreamNames.Accounts.AccountBalanceLedger(DestinationSortCode, DestinationAccountNumber);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}