using AccountsDomain.RequestHandlers.Accounts;

namespace AccountsClient;

public interface IAccountsApiClient
{
    Task<IEnumerable<AccountSummary>> GetAccountSummaries();
}