using System;

namespace events.Payments
{
    public class InboundPaymentReturned_v1 : IEvent
    {
        public string Reason { get; init; }
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public string StreamName() => StreamNames.Accounts.AccountTransactions(DestinationSortCode, DestinationAccountNumber, CorrelationId);
        public int Version() => 1;
    }
}