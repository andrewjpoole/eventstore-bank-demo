using System.Collections.Generic;
using infrastructure.EventStore;

namespace sanctions_api.Services
{
    public interface ISanctionsCatchupHostedService
    {
        List<string> GetSanctionedNames();
    }
}