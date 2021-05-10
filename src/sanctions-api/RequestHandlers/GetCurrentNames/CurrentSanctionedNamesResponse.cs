using System.Collections.Generic;

namespace sanctions_api.RequestHandlers.GetCurrentNames
{
    public class CurrentSanctionedNamesResponse
    {
        public List<string> SanctionedNames { get; init; }
    }
}