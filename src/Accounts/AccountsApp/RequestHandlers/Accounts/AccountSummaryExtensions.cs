using System;
using System.Linq.Expressions;
using AccountsDomain.Events;

namespace AccountsApp.RequestHandlers.Accounts;

public static class AccountSummaryExtensions
{
    public static AccountSummary WithStatus(this AccountSummary existing, AccountStatus newStatus)
    {
        return new AccountSummary
        {
            Id = existing.Id,
            SortCode = existing.SortCode,
            AccountNumber = existing.AccountNumber,
            AccountName = existing.AccountName,
            Opened = existing.Opened,
            Status = newStatus,
        };
    }

    public static AccountSummary With(this AccountSummary existing, Expression<Func<AccountSummary, object>> selector, object newValue)
    {
        var operand = ((UnaryExpression) selector.Body).Operand;
        var prop = ((MemberExpression) operand).Member.Name;
            
        return new AccountSummary
        {
            Id = prop == nameof(existing.Id) ? (Guid)newValue : existing.Id,
            SortCode = prop == nameof(existing.SortCode) ? (int)newValue : existing.SortCode,
            AccountNumber = prop == nameof(existing.AccountNumber) ? (int)newValue : existing.AccountNumber,
            AccountName = prop == nameof(existing.AccountName) ? (string)newValue : existing.AccountName,
            Opened = prop == nameof(existing.Opened) ? (DateTime)newValue : existing.Opened,
            Status = prop == nameof(existing.Status) ? (AccountStatus)newValue : existing.Status
        };
    }
}