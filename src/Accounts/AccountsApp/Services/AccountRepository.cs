using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccountsApp.RequestHandlers.Accounts;
using AccountsDomain.Events;
using Domain.Interfaces;

namespace AccountsApp.Services;

public class AccountRepository : IAccountRepository
{
    private readonly IAccountsCatchupHostedService _accountsCatchupHostedService;
    private readonly IEventPublisher _eventPublisher;
        
    public AccountRepository(IAccountsCatchupHostedService accountsCatchupHostedService, IEventPublisher eventPublisher)
    {
        _accountsCatchupHostedService = accountsCatchupHostedService;
        _eventPublisher = eventPublisher;
    }
        
    public AccountSummary GetById(Guid id) => _accountsCatchupHostedService.Accounts.ContainsKey(id) ? _accountsCatchupHostedService.Accounts[id] : null;

    public async Task<IEnumerable<AccountSummary>> GetAll(string sortcode = "", AccountStatus statusFilter = AccountStatus.Any)
    {

        // TODO add balance filtering

        var filteredAccounts = _accountsCatchupHostedService.Accounts.Select(x => x.Value);
            
        if (!string.IsNullOrEmpty(sortcode))
            filteredAccounts = filteredAccounts.Where(x => x.SortCode.ToString().StartsWith(sortcode)).ToList();

        //if (balanceCriteria != null)
        //    filteredAccounts = filteredAccounts.Where(x => balanceCriteria(x.Balance) == true).ToList();

        if (statusFilter != AccountStatus.Any)
            filteredAccounts = filteredAccounts.Where(x => x.Status == statusFilter);

        return filteredAccounts;
    }

    public async Task<bool> Create(AccountDetails newAccount)
    {
        // Add some validation here
        // ToDo the validation business logic should exist on the event
            
        // raise event to create
        var accountOpened = new AccountOpenedEvent_v1
        {
            Id = Guid.NewGuid(),
            SortCode = newAccount.SortCode,
            AccountNumber = newAccount.AccountNumber,
            Name = newAccount.AccountName,
            Opened = DateTime.Now,
            Status = AccountStatus.Opened
        };

        var success = await _eventPublisher.Publish<AccountOpenedEvent_v1>(accountOpened, accountOpened.StreamName(), CancellationToken.None);

        return success;
    }
        
    public async Task<bool> ChangeStatus(Guid id, AccountStatus status)
    {
        if (_accountsCatchupHostedService.Accounts.ContainsKey(id) == false)
            return false;

        var existingAccount = _accountsCatchupHostedService.Accounts[id];

        // raise event to update status
        var accountUpdated = new AccountStatusUpdated_v1
        {
            Id = existingAccount.Id,
            SortCode = existingAccount.SortCode,
            AccountNumber = existingAccount.AccountNumber,
            Status = status
        };

        var success = await _eventPublisher.Publish(accountUpdated, accountUpdated.StreamName(), CancellationToken.None);

        return success;
    }
}