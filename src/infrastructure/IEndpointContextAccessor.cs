using Microsoft.AspNetCore.Http;

namespace Infrastructure;

public interface IEndpointContextAccessor
{
    HttpContext CurrentContext { get; set; }
}