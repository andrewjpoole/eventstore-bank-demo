using System.Threading;
using System.Threading.Tasks;
using accounts_api.Services;
using AJP.MediatrEndpoints.Exceptions;
using AJP.MediatrEndpoints.Swagger.Attributes;
using MediatR;
using Microsoft.Extensions.Logging;

namespace accounts_api.RequestHandlers.Accounts
{
    public class DeleteAccountByIdRequest : IRequest<AccountDeletedResponse>
    {
        [SwaggerRouteParameter]
        public string Id { get; set; }
    }
    
    public class AccountDeletedResponse
    {
    }
    
    public class DeleteAccountByIdRequestHandler : IRequestHandler<DeleteAccountByIdRequest, AccountDeletedResponse>
    {
        private readonly ILogger<DeleteAccountByIdRequestHandler> _logger;
        private readonly IAccountRepository _accountRepository;
        private readonly IEndpointContextAccessor _endpointContextAccessor;

        public DeleteAccountByIdRequestHandler(ILogger<DeleteAccountByIdRequestHandler> logger, IAccountRepository accountRepository, IEndpointContextAccessor endpointContextAccessor)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _endpointContextAccessor = endpointContextAccessor;
        }
        
        public Task<AccountDeletedResponse> Handle(DeleteAccountByIdRequest request, CancellationToken cancellationToken)
        {
            var deletedId = _accountRepository.Delete(request.Id);

            if (string.IsNullOrEmpty(deletedId))
                throw new NotFoundHttpException($"account with id:{request.Id} not found",
                    "resource not found");
                    
            var correlationId = _endpointContextAccessor.CurrentContext.Request.Headers["CorrelationId"];
            _logger.LogInformation($"Deleted account {request.Id} from request with CorrelationId: {correlationId}");
            
            return Task.FromResult(new AccountDeletedResponse());
        }
    }
}