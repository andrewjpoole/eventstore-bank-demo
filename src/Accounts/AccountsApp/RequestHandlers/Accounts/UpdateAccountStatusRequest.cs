using AccountsDomain.Events;
using AJP.MediatrEndpoints.Swagger.Attributes;
using MediatR;

namespace AccountsApp.RequestHandlers.Accounts;

public class UpdateAccountStatusRequest : IRequest<UpdateAccountStatusResponse>
{
    [SwaggerRouteParameter]
    public string Id { get; init; }

    [SwaggerQueryParameter]
    public AccountStatus Status { get; init; }
}