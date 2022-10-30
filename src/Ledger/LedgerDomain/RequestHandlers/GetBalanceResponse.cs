namespace LedgerDomain.RequestHandlers;

public class GetBalanceResponse
{
    public GetBalanceResponse(int sortCode, int accountNumber, decimal accountBalance)
    {
        SortCode = sortCode;
        AccountNumber = accountNumber;
        AccountBalance = accountBalance;
    }

    public int SortCode { get; init; }
    public int AccountNumber { get; init; }
    public decimal AccountBalance { get; init; }
}