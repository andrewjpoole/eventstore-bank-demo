using AccountsDomain.Events;
using AccountsDomain.RequestHandlers.Accounts;

namespace AccountsDomain.ServiceInterfaces;

public interface IAccountRepository
{
    AccountSummary GetById(Guid id);
    //Task<IEnumerable<AccountDetails>> GetAll(string sortcode = "", Func<decimal, bool> balanceCriteria = null, events.Accounts.AccountStatus statusFilter = events.Accounts.AccountStatus.Any);
    Task<IEnumerable<AccountSummary>> GetAll(string sortcode = "", AccountStatus statusFilter = AccountStatus.Any);
    Task<bool> Create(AccountDetails newAccount);
    Task<bool> ChangeStatus(Guid id, AccountStatus status);
}