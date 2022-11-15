using AccountsDomain.ServiceInterfaces;
using AJP.MediatrEndpoints.Exceptions;
using MediatR;

namespace AccountsDomain.RequestHandlers.Accounts;

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