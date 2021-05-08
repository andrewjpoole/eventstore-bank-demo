using System;

namespace events.Accounts
{
    public class AccountOpenedEvent_v1 : IEvent
    {
        public string Id { get; init; }
        public int SortCode { get; init; }
        public int AccountNumber { get; init; }
        public string Name { get; init; }
        public AccountStatus Status { get; init; }
        public DateTime Opened { get; init; }

        public string StreamName() => SubscriptionNames.Accounts.AccountOpened(SortCode, AccountNumber);
        public int Version() => 1;
    }
}
