using AccountsDomain.RequestHandlers.Accounts;

namespace AccountsDomain.ServiceInterfaces;

public interface IAccountsCatchupHostedService
{
    Dictionary<Guid, AccountSummary> Accounts { get; }
}