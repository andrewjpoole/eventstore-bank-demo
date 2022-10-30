using MediatR;

namespace LedgerDomain.RequestHandlers;

public class GetBalanceRequest : IRequest<GetBalanceResponse>
{
    public GetBalanceRequest(int sortCode, int accountNumber)
    {
        SortCode = sortCode;
        AccountNumber = accountNumber;
    }

    public int SortCode { get; init; }
    public int AccountNumber { get; init; }
}