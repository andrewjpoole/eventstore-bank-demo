using System.Collections.Generic;
using infrastructure.EventStore;

namespace sanctions_api.Services
{
    public interface ISanctionsCatchupHostedService//<T> : ICatchupSubscription<T> where T : new()
    {
        List<string> GetSanctionedNames();
    }
}