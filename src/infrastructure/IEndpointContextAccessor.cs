using Microsoft.AspNetCore.Http;

namespace infrastructure
{
    public interface IEndpointContextAccessor
    {
        HttpContext CurrentContext { get; set; }
    }
}