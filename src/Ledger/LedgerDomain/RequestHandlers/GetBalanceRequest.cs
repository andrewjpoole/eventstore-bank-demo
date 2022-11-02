using MediatR;

namespace LedgerDomain.RequestHandlers;

public class GetBalanceRequest : IRequest<GetBalanceResponse>
{
    public int SortCode { get; set; }
    
    public int AccountNumber { get; set; }
}