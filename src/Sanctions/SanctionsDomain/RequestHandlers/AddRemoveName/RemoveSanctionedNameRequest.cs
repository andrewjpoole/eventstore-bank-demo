using MediatR;

namespace SanctionsDomain.RequestHandlers.AddRemoveName;

public class RemoveSanctionedNameRequest : IRequest<SanctionedNameChangeResponse>
{
    public string Name { get; init; }
}