using System;
using System.Collections.Generic;
using accounts_api.RequestHandlers.Accounts;

namespace accounts_api.Services
{
    public interface IAccountsCatchupHostedService
    {
        Dictionary<Guid, AccountSummary> Accounts { get; }
    }
}