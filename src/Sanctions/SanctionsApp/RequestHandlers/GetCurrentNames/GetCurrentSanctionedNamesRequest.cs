using MediatR;

namespace sanctions_api.RequestHandlers.GetCurrentNames;

public class GetCurrentSanctionedNamesRequest : IRequest<CurrentSanctionedNamesResponse>
{
}