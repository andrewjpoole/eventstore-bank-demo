using AccountsDomain.Events;
using AccountsDomain.ServiceInterfaces;
using MediatR;

namespace AccountsDomain.RequestHandlers.Accounts;

public class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest, CreateAccountResponse>
{
    private readonly IAccountRepository _accountRepository;

    public CreateAccountRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }
        
    public async Task<CreateAccountResponse> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var success = await _accountRepository.Create(new AccountDetails
        {
            SortCode = request.SortCode,
            AccountNumber = request.AccountNumber,
            AccountName = request.Name,
            Status = AccountStatus.Opened,
            Opened = DateTime.Now
        });
        return new CreateAccountResponse();
    }
}