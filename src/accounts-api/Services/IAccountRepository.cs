using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;

namespace accounts_api.Services
{
    public interface IAccountRepository
    {
        AccountDetails GetById(string id);
        Task<IEnumerable<AccountDetails>> GetAll(string sortcode = "", Func<decimal, bool> balanceCriteria = null, events.Accounts.AccountStatus statusFilter = events.Accounts.AccountStatus.Any);
        AccountDetails Create(AccountDetails newAccount);
        AccountDetails ChangeStatus(string id, events.Accounts.AccountStatus status);
    }
}