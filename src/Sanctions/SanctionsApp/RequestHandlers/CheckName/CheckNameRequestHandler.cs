using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using sanctions_api.Services;

namespace sanctions_api.RequestHandlers.CheckName;

public class CheckNameRequestHandler : IRequestHandler<CheckNameRequest, CheckNameResponse>
{
    private readonly ISanctionedNamesCatchupHostedService _sanctionedNamesSubscriptionHostedService;

    public CheckNameRequestHandler(ISanctionedNamesCatchupHostedService sanctionedNamesSubscriptionHostedService)
    {
        _sanctionedNamesSubscriptionHostedService = sanctionedNamesSubscriptionHostedService;
    }

    public Task<CheckNameResponse> Handle(CheckNameRequest request, CancellationToken cancellationToken)
    {
        var isSanctioned = _sanctionedNamesSubscriptionHostedService.GetSanctionedNames().Select(x => x.ToLower()).Contains(request.Name.ToLower());

        return Task.FromResult(new CheckNameResponse
        {
            Name = request.Name,
            IsSanctioned = isSanctioned
        });
    }
}