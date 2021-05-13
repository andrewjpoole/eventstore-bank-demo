using System.Collections.Generic;

namespace accounts_api.Services
{
    public interface IAccountsCatchupHostedService
    {
        List<string> GetAccounts();
    }
}