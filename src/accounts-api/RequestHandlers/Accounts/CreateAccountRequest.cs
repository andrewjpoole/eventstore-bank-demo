using System;
using System.Threading;
using System.Threading.Tasks;
using accounts_api.Services;
using events.Accounts;
using MediatR;

namespace accounts_api.RequestHandlers.Accounts
{
    public class CreateAccountRequest : IRequest<CreateAccountResponse>
    {
        public int SortCode { get; init; }
        public int AccountNumber { get; init; }
        public string Name { get; init; }
    }
    
    public class CreateAccountResponse
    {
    }
    
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
}