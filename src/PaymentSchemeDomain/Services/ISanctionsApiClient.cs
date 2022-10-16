using System.Threading.Tasks;
using OneOf;
using OneOf.Types;

namespace payment_scheme_domain.Services;

public interface ISanctionsApiClient
{
    Task<bool> CheckIfNameIsSanctioned(string name);
    Task<OneOf<False, string>> CheckIfNameIsSanctioned2(string name);
}