using System.Threading;
using System.Threading.Tasks;
using accounts_api.Services;
using AJP.MediatrEndpoints.Exceptions;
using AJP.MediatrEndpoints.Swagger.Attributes;
using MediatR;

namespace accounts_api.RequestHandlers.Accounts
{
    public class UpdateAccountStatusRequest : IRequest<AccountDetails>
    {
        [SwaggerRouteParameter]
        public string Id { get; set; }
        public bool Blocked { get; set; }
    }
    
    public class UpdateAccountStatusRequestHandler : IRequestHandler<UpdateAccountStatusRequest, AccountDetails>
    {
        private readonly IAccountRepository _accountRepository;

        public UpdateAccountStatusRequestHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public Task<AccountDetails> Handle(UpdateAccountStatusRequest request, CancellationToken cancellationToken)
        {
            var account = _accountRepository.GetById(request.Id);

            if (account == null)
                throw new NotFoundHttpException($"account with id:{request.Id} not found", "resource not found");
            
            return request.Blocked switch
            {
                true => Task.FromResult(_accountRepository.Block(account.Id)),
                false => Task.FromResult(_accountRepository.UnBlock(account.Id))
            };
        }
    }
}