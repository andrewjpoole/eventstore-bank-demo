using System;
using System.Threading;
using System.Threading.Tasks;
using accounts_api.Services;
using AJP.MediatrEndpoints.Exceptions;
using AJP.MediatrEndpoints.Swagger.Attributes;
using Domain.Events.Accounts;
using MediatR;

namespace accounts_api.RequestHandlers.Accounts;

public class UpdateAccountStatusRequest : IRequest<UpdateAccountStatusResponse>
{
    [SwaggerRouteParameter]
    public string Id { get; init; }

    [SwaggerQueryParameter]
    public AccountStatus Status { get; init; }
}

public class UpdateAccountStatusResponse { }

public class UpdateAccountStatusRequestHandler : IRequestHandler<UpdateAccountStatusRequest, UpdateAccountStatusResponse>
{
    private readonly IAccountRepository _accountRepository;

    public UpdateAccountStatusRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
        
    public async Task<UpdateAccountStatusResponse> Handle(UpdateAccountStatusRequest request, CancellationToken cancellationToken)
    {
        var account = _accountRepository.GetById(Guid.Parse(request.Id));

        if (account == null)
            throw new NotFoundHttpException($"account with id:{request.Id} not found", "resource not found");
            
        var success = await _accountRepository.ChangeStatus(account.Id, request.Status);
        return new UpdateAccountStatusResponse();
    }
}