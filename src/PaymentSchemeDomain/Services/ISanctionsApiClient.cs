using System.Threading.Tasks;
using OneOf;
using OneOf.Types;

namespace payment_scheme_domain.Services;

public interface ISanctionsApiClient
{
    Task<OneOf<False, string>> CheckIfNameIsSanctioned(string? name);
}