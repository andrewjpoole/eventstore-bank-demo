using System.Threading.Tasks;

namespace payment_scheme_domain.Services
{
    public interface ISanctionsApiClient
    {
        Task<bool> CheckIfNameIsSanctioned(string name);
    }
}