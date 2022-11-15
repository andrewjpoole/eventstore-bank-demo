using MediatR;

namespace AccountsDomain.RequestHandlers.Accounts;

public class CreateAccountRequest : IRequest<CreateAccountResponse>
{
    public int SortCode { get; init; }
    public int AccountNumber { get; init; }
    public string Name { get; init; }
}