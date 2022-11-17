using MediatR;

namespace SanctionsDomain.RequestHandlers.AddRemoveName;

public class AddSanctionedNameRequest : IRequest<SanctionedNameChangeResponse>
{
    public string Name { get; init; }
}