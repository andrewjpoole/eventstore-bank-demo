using System.Threading;
using System.Threading.Tasks;
using MediatR;
using sanctions_api.Services;

namespace sanctions_api.RequestHandlers.GetCurrentNames
{
    public class GetCurrentSanctionedNamesRequestHandler : IRequestHandler<GetCurrentSanctionedNamesRequest, CurrentSanctionedNamesResponse>
    {
        private readonly ISanctionsCatchupHostedService _sanctionedNamesSubscriptionHostedService;

        public GetCurrentSanctionedNamesRequestHandler(ISanctionsCatchupHostedService sanctionedNamesSubscriptionHostedService)
        {
            _sanctionedNamesSubscriptionHostedService = sanctionedNamesSubscriptionHostedService;
        }

        public async Task<CurrentSanctionedNamesResponse> Handle(GetCurrentSanctionedNamesRequest request, CancellationToken cancellationToken)
        {
            return new CurrentSanctionedNamesResponse
            {
                SanctionedNames = _sanctionedNamesSubscriptionHostedService.GetSanctionedNames()
            };
        }
    }
}