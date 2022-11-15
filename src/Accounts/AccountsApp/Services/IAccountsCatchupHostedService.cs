using System;
using System.Collections.Generic;
using AccountsApp.RequestHandlers.Accounts;

namespace AccountsApp.Services;

public interface IAccountsCatchupHostedService
{
    Dictionary<Guid, AccountSummary> Accounts { get; }
}