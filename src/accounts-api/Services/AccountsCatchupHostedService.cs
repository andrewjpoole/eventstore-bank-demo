using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;
using events;
using events.Accounts;
using EventStore.Client;
using infrastructure.EventStore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace accounts_api.Services
{
    public class AccountsCatchupHostedService : BackgroundService, IAccountsCatchupHostedService
    {
        private readonly ILogger<AccountsCatchupHostedService> _logger;
        private readonly ICatchupSubscription _catchupSubscription;
        private readonly Dictionary<Guid,AccountSummary> _accounts = new();

        public AccountsCatchupHostedService(ILogger<AccountsCatchupHostedService> logger, ICatchupSubscription catchupCatchupSubscription)
        {
            _logger = logger;
            _catchupSubscription = catchupCatchupSubscription;
        }

        public Dictionary<Guid, AccountSummary> Accounts => _accounts;

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return _catchupSubscription.StartAsync(StreamNames.Accounts.AllAccounts, "AccountsCatchupHostedService", cancellationToken,
                (subscription, @event, json, ct) =>
                {
                    _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType}");
                    return @event.Event.EventType switch
                    {
                        nameof(AccountOpenedEvent_v1) => HandleAccountOpenedEvent(@event, json),
                        nameof(AccountStatusUpdated_v1) => HandleAccountStatusUpdatedEvent(@event, json),
                        _ => throw new NotImplementedException()
                    };
                });
        }

        private Task HandleAccountOpenedEvent(ResolvedEvent @event, string json)
        {
            var eventData = JsonSerializer.Deserialize<AccountOpenedEvent_v1>(json);

            if (eventData is null || eventData.Id == Guid.Empty)
            {
                _logger.LogWarning($"eventData is null or missing or malformed, ignoring event {StreamNames.Sanctions.GlobalSanctionedNames}#{@event.OriginalEventNumber}");
                return Task.CompletedTask;
            }

            if (!_accounts.ContainsKey(eventData.Id))
                _accounts.Add(eventData.Id, new AccountSummary
                {
                    Id = eventData.Id,
                    SortCode = eventData.SortCode,
                    AccountNumber = eventData.AccountNumber,
                    AccountName = eventData.Name,
                    Status = eventData.Status,
                    Opened = eventData.Opened
                });

            return Task.CompletedTask;
        }

        private Task HandleAccountStatusUpdatedEvent(ResolvedEvent @event, string json)
        {
            var eventData = JsonSerializer.Deserialize<AccountStatusUpdated_v1>(json);

            if (eventData is null || eventData.Id == Guid.Empty)
            {
                _logger.LogWarning(
                    $"eventData is null or missing a sanctioned name to add, ignoring event {StreamNames.Sanctions.GlobalSanctionedNames}#{@event.OriginalEventNumber}");
                return Task.CompletedTask;
            }

            var account = _accounts[eventData.Id];
            if (account is null)
            {
                //log
                //?
            }

            //var accountWithNewStatus = account.WithStatus(eventData.Status);
            var accountWithNewStatus = account.With(a => a.Status, eventData.Status);
            _accounts[eventData.Id] = accountWithNewStatus;

            return Task.CompletedTask;
        }
    }
}