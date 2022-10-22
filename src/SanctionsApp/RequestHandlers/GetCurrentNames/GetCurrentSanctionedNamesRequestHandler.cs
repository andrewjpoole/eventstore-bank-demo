using System.Threading;
using System.Threading.Tasks;
using MediatR;
using sanctions_api.Services;

namespace sanctions_api.RequestHandlers.GetCurrentNames;

public class GetCurrentSanctionedNamesRequestHandler : IRequestHandler<GetCurrentSanctionedNamesRequest, CurrentSanctionedNamesResponse>
{
    private readonly ISanctionedNamesCatchupHostedService _sanctionedNamesSubscriptionHostedService;

    public GetCurrentSanctionedNamesRequestHandler(ISanctionedNamesCatchupHostedService sanctionedNamesSubscriptionHostedService)
    {
        _sanctionedNamesSubscriptionHostedService = sanctionedNamesSubscriptionHostedService;
    }

    public Task<CurrentSanctionedNamesResponse> Handle(GetCurrentSanctionedNamesRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CurrentSanctionedNamesResponse
        {
            SanctionedNames = _sanctionedNamesSubscriptionHostedService.GetSanctionedNames()
        });
    }
}