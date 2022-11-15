using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountsApp.Services;
using MediatR;

namespace AccountsApp.RequestHandlers.Accounts;

public class GetAccountsRequestHandler : IRequestHandler<GetAccountsRequest, IEnumerable<AccountSummary>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
        
    public async Task<IEnumerable<AccountSummary>> Handle(GetAccountsRequest request, CancellationToken cancellationToken)
    {
        //return WebUtility.UrlDecode(request.BalanceFilter) switch
        //{
        //    string s when s.StartsWith("<") => _accountRepository.GetAll(request.SortCodeMatch,
        //            balance => balance < decimal.Parse(s.Remove(0, 1)), request.AccountStatusFilter),
        //    string s when s.StartsWith(">") => _accountRepository.GetAll(request.SortCodeMatch,
        //            balance => balance > decimal.Parse(s.Remove(0, 1)), request.AccountStatusFilter),
        //    _ => _accountRepository.GetAll(request.SortCodeMatch, null, request.AccountStatusFilter)
        //};
        var accountSummaries = await _accountRepository.GetAll(request.SortCodeMatch, request.AccountStatusFilter);
        return accountSummaries.ToList();
    }
}