﻿using AJP.MediatrEndpoints.Swagger.Attributes;
using MediatR;

namespace AccountsApp.RequestHandlers.Accounts;

public class GetAccountByIdRequest : IRequest<AccountSummary>
{
    [SwaggerRouteParameter]
    public string Id { get; set; }
}