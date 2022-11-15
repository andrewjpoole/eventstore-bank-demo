using System.Collections.Generic;
using AccountsDomain.Events;
using AJP.MediatrEndpoints.PropertyAttributes;
using AJP.MediatrEndpoints.Swagger.Attributes;
using MediatR;

namespace AccountsApp.RequestHandlers.Accounts;

[SwaggerDescription("Method which gets account details")]
public class GetAccountsRequest : IRequest<IEnumerable<AccountSummary>>
{
    [OptionalProperty]
    //[SwaggerExampleValue("2099")]
    [SwaggerDescription("Use this optional parameter to filter results by sortcode, uses StartsWith")]
    public string SortCodeMatch { get; set; }

    [OptionalProperty]
    //[SwaggerExampleValue("<-1000")]
    [SwaggerDescription("Use this optional parameter to filter results by balance e.g. <-1000 will return accounts with a balance of less than £-1000")]
    public string BalanceFilter { get; set; }

    [OptionalProperty] 
    public AccountStatus AccountStatusFilter { get; set; } = AccountStatus.Any;
}