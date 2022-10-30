using System.Threading;
using System.Threading.Tasks;
using MediatR;
using sanctions_api.Services;

namespace sanctions_api.RequestHandlers.HeldPayments;

public class GetHeldPaymentsRequestHandler : IRequestHandler<GetHeldPaymentsRequest, GetHeldPaymentsResponse>
{
    private readonly IHeldPaymentsCatchupHostedService _heldPaymentsCatchupHostedService;

    public GetHeldPaymentsRequestHandler(IHeldPaymentsCatchupHostedService heldPaymentsCatchupHostedService)
    {
        _heldPaymentsCatchupHostedService = heldPaymentsCatchupHostedService;
    }

    public Task<GetHeldPaymentsResponse> Handle(GetHeldPaymentsRequest request, CancellationToken cancellationToken)
    {
        var response = new GetHeldPaymentsResponse
        {
            HeldPayments = _heldPaymentsCatchupHostedService.GetHeldPayments(request.ExcludeReleasedPayments)
        };
        return Task.FromResult(response);
    }
}