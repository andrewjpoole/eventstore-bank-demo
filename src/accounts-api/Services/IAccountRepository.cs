using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;

namespace accounts_api.Services
{
    public interface IAccountRepository
    {
        AccountDetails GetById(string id);
        Task<IEnumerable<AccountDetails>> GetAll(string sortcode = "", Func<decimal, bool> balanceCriteria = null, AccountStatus statusFilter = AccountStatus.Any);
        AccountDetails Create(AccountDetails newAccount);
        AccountDetails Block(string id);
        AccountDetails UnBlock(string id);
    }
}