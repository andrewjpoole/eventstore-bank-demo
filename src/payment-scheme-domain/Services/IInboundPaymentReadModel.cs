using System;
using System.Threading;
using System.Threading.Tasks;
using events.Payments;

namespace payment_scheme_domain.Services
{
    public interface IInboundPaymentReadModel
    {
        int SortCode { get; }
        int AccountNumber { get; }
        Guid CorrelationId { get; }
        int OriginatingSortCode { get; }
        int OriginatingAccountNumber { get; }
        string OriginatingAccountName { get; }
        string PaymentReference { get; }
        string DestinationAccountName { get; }
        decimal Amount { get; }
        PaymentScheme Scheme { get; }
        PaymentType Type { get; }
        DateTime ProcessingDate { get; }
        bool PaymentValidated { get; }
        bool PassedSanctionsCheck { get; }
        bool PassedAccountStatusCheck { get; }
        bool FundsCleared { get; }
        Guid ClearedTransactionId { get; }
        Task Read(int sortCode, int accountNumber, Guid correlationId, CancellationToken cancellationToken);
    }
}