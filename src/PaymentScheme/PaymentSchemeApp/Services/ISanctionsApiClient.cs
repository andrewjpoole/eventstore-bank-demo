using System.Threading.Tasks;
using OneOf;
using OneOf.Types;

namespace PaymentSchemeApp.Services;

public interface ISanctionsApiClient
{
    Task<OneOf<False, string>> CheckIfNameIsSanctioned(string? name);
}