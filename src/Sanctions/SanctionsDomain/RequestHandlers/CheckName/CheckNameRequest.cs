using MediatR;

namespace SanctionsDomain.RequestHandlers.CheckName;

public class CheckNameRequest : IRequest<CheckNameResponse>
{
    public string Name { get; init; }
}