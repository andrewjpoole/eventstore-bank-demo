using MediatR;

namespace LedgerDomain.RequestHandlers;

public class PostLedgerEntryRequest :IRequest<PostLedgerEntryResponse>
{
    public PostLedgerEntryRequest(string reference, int destinationSortCode, int destinationAccountNumber, int originatingSortCode, int originatingAccountNumber, Guid paymentId, Guid correlationId, decimal amount)
    {
        Reference = reference;
        DestinationSortCode = destinationSortCode;
        DestinationAccountNumber = destinationAccountNumber;
        OriginatingSortCode = originatingSortCode;
        OriginatingAccountNumber = originatingAccountNumber;
        PaymentId = paymentId;
        CorrelationId = correlationId;
        Amount = amount;
    }

    public Guid PaymentId { get; init; }
    public Guid CorrelationId { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public int OriginatingSortCode { get; init; }
    public int OriginatingAccountNumber { get; init; }
    public string Reference { get; init; }
    public decimal Amount { get; init; }
}