using System;
using AccountsDomain.Events;

namespace AccountsApp.RequestHandlers.Accounts;

public class AccountSummary
{
    public Guid Id { get; set; }
    public int SortCode { get; init; }
    public int AccountNumber { get; init; }
    public AccountStatus Status { get; init; }
    public string AccountName { get; init; }
    public DateTime Opened { get; init; }
}