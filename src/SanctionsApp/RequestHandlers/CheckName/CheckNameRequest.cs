using MediatR;

namespace sanctions_api.RequestHandlers.CheckName;

public class CheckNameRequest : IRequest<CheckNameResponse>
{
    public string Name { get; init; }
}