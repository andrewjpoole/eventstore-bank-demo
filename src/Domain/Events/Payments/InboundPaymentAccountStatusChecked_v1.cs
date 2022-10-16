using System;
using System.Collections.Generic;
using OneOf;
using OneOf.Types;

namespace Domain.Events.Payments;

public class InboundPaymentAccountStatusChecked_v1 : IEvent
{
    public Guid CorrelationId { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber, CorrelationId);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}