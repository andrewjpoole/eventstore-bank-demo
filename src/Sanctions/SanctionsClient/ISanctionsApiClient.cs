using OneOf;
using OneOf.Types;

namespace SanctionsClient;

public interface ISanctionsApiClient
{
    Task<OneOf<False, string>> CheckIfNameIsSanctioned(string? name);
}