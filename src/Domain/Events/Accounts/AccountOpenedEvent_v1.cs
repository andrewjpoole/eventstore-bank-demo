﻿using System;
using System.Collections.Generic;
using OneOf;
using OneOf.Types;

namespace Domain.Events.Accounts;

public class AccountOpenedEvent_v1 : IEvent
{
    public Guid Id { get; init; }
    public int SortCode { get; init; }
    public int AccountNumber { get; init; }
    public string Name { get; init; }
    public AccountStatus Status { get; init; }
    public DateTime Opened { get; init; }

    public string StreamName() => StreamNames.Accounts.AccountDetails(SortCode, AccountNumber);
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}