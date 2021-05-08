using Microsoft.AspNetCore.Http;

namespace accounts_api.Services
{
    public class EndpointContextAccessor : IEndpointContextAccessor
    {
        public HttpContext CurrentContext { get; set; } // register as scoped/one per request
    }
}