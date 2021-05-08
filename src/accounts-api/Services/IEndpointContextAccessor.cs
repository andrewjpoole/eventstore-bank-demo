using Microsoft.AspNetCore.Http;

namespace accounts_api.Services
{
    public interface IEndpointContextAccessor
    {
        HttpContext CurrentContext { get; set; }
    }
}