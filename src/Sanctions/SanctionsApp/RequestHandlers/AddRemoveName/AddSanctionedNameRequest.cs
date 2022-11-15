using MediatR;

namespace sanctions_api.RequestHandlers.AddRemoveName;

public class AddSanctionedNameRequest : IRequest<SanctionedNameChangeResponse>
{
    public string Name { get; init; }
}