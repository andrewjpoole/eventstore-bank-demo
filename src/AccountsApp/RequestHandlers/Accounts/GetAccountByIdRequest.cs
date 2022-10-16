﻿using System;
using System.Threading;
using System.Threading.Tasks;
using accounts_api.Services;
using AJP.MediatrEndpoints.Exceptions;
using AJP.MediatrEndpoints.Swagger.Attributes;
using MediatR;

namespace accounts_api.RequestHandlers.Accounts;

public class GetAccountByIdRequest : IRequest<AccountSummary>
{
    [SwaggerRouteParameter]
    public string Id { get; set; }
}
    
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