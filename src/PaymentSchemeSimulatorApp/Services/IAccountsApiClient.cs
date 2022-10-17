using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;

namespace payment_scheme_simulator.Services;

public interface IAccountsApiClient
{
    Task<IEnumerable<AccountSummary>> GetAccountSummaries();
}