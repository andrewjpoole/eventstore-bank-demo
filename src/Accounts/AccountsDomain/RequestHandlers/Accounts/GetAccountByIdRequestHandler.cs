using AccountsDomain.ServiceInterfaces;
using AJP.MediatrEndpoints.Exceptions;
using MediatR;

namespace AccountsDomain.RequestHandlers.Accounts;

public class GetAccountByIdRequestHandler : IRequestHandler<GetAccountByIdRequest, AccountSummary>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
        
    public Task<AccountSummary> Handle(GetAccountByIdRequest request, CancellationToken cancellationToken)
    {
        var account = _accountRepository.GetById(Guid.Parse(request.Id));

        if (account == null)
            throw new NotFoundHttpException($"account with id:{request.Id} not found", "resource not found");
            
        return Task.FromResult(account);
    }
}