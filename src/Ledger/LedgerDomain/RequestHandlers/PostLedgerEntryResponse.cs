namespace LedgerDomain.RequestHandlers;

public class PostLedgerEntryResponse
{
    public PostLedgerEntryResponse(int sortCode, int accountNumber, Guid transactionId)
    {
        SortCode = sortCode;
        AccountNumber = accountNumber;
        TransactionId = transactionId;
    }

    public int SortCode { get; init; }
    public int AccountNumber { get; init; }
    public Guid TransactionId { get; init; }
}