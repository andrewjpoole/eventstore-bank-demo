using MediatR;

namespace SanctionsDomain.RequestHandlers.GetCurrentNames;

public class GetCurrentSanctionedNamesRequest : IRequest<CurrentSanctionedNamesResponse>
{
}