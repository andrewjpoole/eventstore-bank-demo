using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;
using events.Accounts;
using infrastructure.EventStore;
using AccountStatus = accounts_api.RequestHandlers.Accounts.AccountStatus;

namespace accounts_api.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IEventStreamReader<AccountOpenedEvent_v1> _accountOpenedReader;
        private readonly Dictionary<string, AccountDetails> _accounts;

        public AccountRepository(IEventStreamReader<AccountOpenedEvent_v1> accountOpenedReader)
        {
            _accountOpenedReader = accountOpenedReader;
        }
        
        public AccountDetails GetById(string id) => _accounts.ContainsKey(id) ? _accounts[id] : null;

        public async Task<IEnumerable<AccountDetails>> GetAll(string sortcode = "", Func<decimal, bool> balanceCriteria = null, AccountStatus statusFilter = AccountStatus.Any)
        {
            var allOpenededAccounts = await _accountOpenedReader.ReadEventsFromStream("$et-AccountOpenedEvent_v1");

            // TODO start here

            var filteredAccounts = _accounts.Select(x => x.Value);
            
            if (!string.IsNullOrEmpty(sortcode))
                filteredAccounts = filteredAccounts.Where(x => x.SortCode.StartsWith(sortcode)).ToList();

            if (balanceCriteria != null)
                filteredAccounts = filteredAccounts.Where(x => balanceCriteria(x.Balance) == true).ToList();

            if (statusFilter != AccountStatus.Any)
                filteredAccounts = filteredAccounts.Where(x => x.Status == statusFilter);

            return filteredAccounts;
        }

        public AccountDetails Create(AccountDetails newAccount)
        {
            // Add some validation here
            
            var newId = Guid.NewGuid().ToString();
            newAccount.Id = newId;
            _accounts.Add(newId, newAccount);
            return _accounts[newId];
        }
        
        public AccountDetails Block(string id)
        {
            if (_accounts.ContainsKey(id) == false)
                return null;

            _accounts[id].Status = AccountStatus.Blocked;
            return _accounts[id];
        }

        public AccountDetails UnBlock(string id)
        {
            if (!_accounts.ContainsKey(id) == false)
                return null;

            _accounts[id].Status = AccountStatus.Unblocked;
            return _accounts[id];
        }
    }
}