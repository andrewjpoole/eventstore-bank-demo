using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;
using Domain.Events.Accounts;

namespace accounts_api.Services;

public interface IAccountRepository
{
    AccountSummary GetById(Guid id);
    //Task<IEnumerable<AccountDetails>> GetAll(string sortcode = "", Func<decimal, bool> balanceCriteria = null, events.Accounts.AccountStatus statusFilter = events.Accounts.AccountStatus.Any);
    Task<IEnumerable<AccountSummary>> GetAll(string sortcode = "", AccountStatus statusFilter = AccountStatus.Any);
    Task<bool> Create(AccountDetails newAccount);
    Task<bool> ChangeStatus(Guid id, AccountStatus status);
}