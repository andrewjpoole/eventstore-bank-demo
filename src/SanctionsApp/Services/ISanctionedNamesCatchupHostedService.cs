using System.Collections.Generic;

namespace sanctions_api.Services;

public interface ISanctionedNamesCatchupHostedService
{
    List<string> GetSanctionedNames();
}