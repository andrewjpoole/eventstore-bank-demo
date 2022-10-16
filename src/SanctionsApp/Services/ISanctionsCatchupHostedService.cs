using System.Collections.Generic;

namespace sanctions_api.Services;

public interface ISanctionsCatchupHostedService
{
    List<string> GetSanctionedNames();
}