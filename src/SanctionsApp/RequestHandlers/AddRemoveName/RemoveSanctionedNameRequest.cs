using MediatR;

namespace sanctions_api.RequestHandlers.AddRemoveName;

public class RemoveSanctionedNameRequest : IRequest<SanctionedNameChangeResponse>
{
    public string Name { get; init; }
}