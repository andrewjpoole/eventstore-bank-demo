using Domain;
using Domain.Interfaces;
using OneOf;
using OneOf.Types;

namespace LedgerDomain.Events
{
    public record LedgerEntryPosted_v1 : IEvent
    {
        public Guid TransactionId { get; init; }
        public Guid PaymentId { get; init; }
        public Guid CorrelationId { get; init; }
        public int DestinationSortCode { get; init; }
        public int DestinationAccountNumber { get; init; }
        public int OriginatingSortCode { get; init; }
        public int OriginatingAccountNumber { get; init; }
        public string Reference { get; init; }
        public decimal Amount { get; init; }
        public string StreamName() => StreamNames.Ledger.AccountLedger(DestinationSortCode, DestinationAccountNumber);
        public int Version() => 1;
        public OneOf<True, List<string>> IsValid() => new True();
    }
}
