using System;
using AccountsDomain.Events;

namespace AccountsApp.RequestHandlers.Accounts;

public class AccountDetails
{
    public Guid Id { get; set; }
    public int SortCode { get; init; }
    public int AccountNumber { get; init; }
    public decimal Balance { get; init; }
    public AccountStatus Status { get; set; }
    public string AccountName { get; init; }
    public DateTime Opened { get; init; }

    // add more details to this model
}