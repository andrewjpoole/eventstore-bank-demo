using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using accounts_api.Services;
using AJP.MediatrEndpoints.PropertyAttributes;
using AJP.MediatrEndpoints.Swagger.Attributes;
using events.Accounts;
using MediatR;

namespace accounts_api.RequestHandlers.Accounts
{
    [SwaggerDescription("Method which gets account details")]
    public class GetAccountsRequest : IRequest<IEnumerable<AccountSummary>>
    {
        [OptionalProperty]
        [SwaggerExampleValue("2099")]
        [SwaggerDescription("Use this optional parameter to filter results by sortcode, uses StartsWith")]
        public string SortCodeMatch { get; set; }

        //[OptionalProperty]
        //[SwaggerExampleValue("<-1000")]
        //[SwaggerDescription("Use this optional parameter to filter results by balance e.g. <-1000 will return accounts with a balance of less than £-1000")]
        //public string BalanceFilter { get; set; }

        [OptionalProperty] 
        public AccountStatus AccountStatusFilter { get; set; } = AccountStatus.Any;
    }

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
}