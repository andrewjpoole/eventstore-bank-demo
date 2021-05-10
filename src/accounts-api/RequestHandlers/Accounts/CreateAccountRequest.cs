using System.Threading;
using System.Threading.Tasks;
using accounts_api.Services;
using MediatR;

namespace accounts_api.RequestHandlers.Accounts
{
    public class CreateAccountRequest : IRequest<CreateAccountResponse>
    {
        public string SortCode { get; init; }
        public string AccountNumber { get; init; }
        public decimal Balance { get; init; }
    }
    
    public class CreateAccountResponse : AccountDetails
    {
        public CreateAccountResponse(AccountDetails account)
        {
            Id = account.Id;
            SortCode = account.SortCode;
            AccountNumber = account.AccountNumber;
            Balance = account.Balance;
            Status = account.Status;
        }
    }
    
    public class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest, CreateAccountResponse>
    {
        private readonly IAccountRepository _accountRepository;

        public CreateAccountRequestHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public Task<CreateAccountResponse> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var newAccount = _accountRepository.Create(new AccountDetails
            {
                SortCode = request.SortCode,
                AccountNumber = request.AccountNumber,
                Balance = request.Balance,
                Status = AccountStatus.Unblocked
            });
            return Task.FromResult(new CreateAccountResponse(newAccount));
        }
    }
}