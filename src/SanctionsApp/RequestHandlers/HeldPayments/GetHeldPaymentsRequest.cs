using MediatR;
using AJP.MediatrEndpoints.PropertyAttributes;

namespace sanctions_api.RequestHandlers.HeldPayments;

public class GetHeldPaymentsRequest : IRequest<GetHeldPaymentsResponse>
{
    [OptionalProperty]
    public bool ExcludeReleasedPayments { get; set; }
}