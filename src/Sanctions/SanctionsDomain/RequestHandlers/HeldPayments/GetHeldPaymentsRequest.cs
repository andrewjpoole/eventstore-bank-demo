using AJP.MediatrEndpoints.PropertyAttributes;
using MediatR;

namespace SanctionsDomain.RequestHandlers.HeldPayments;

public class GetHeldPaymentsRequest : IRequest<GetHeldPaymentsResponse>
{
    [OptionalProperty]
    public bool ExcludeReleasedPayments { get; set; }
}