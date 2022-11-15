using Infrastructure;
using Microsoft.AspNetCore.Http;

namespace AccountsApp.Services;

public class EndpointContextAccessor : IEndpointContextAccessor
{
    public HttpContext CurrentContext { get; set; } // register as scoped/one per request
}