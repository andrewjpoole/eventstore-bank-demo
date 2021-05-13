using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using events;
using events.Accounts;
using events.Sanctions;
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
        private readonly List<string> _sanctionedNames = new();

        public AccountsCatchupHostedService(ILogger<AccountsCatchupHostedService> logger, ICatchupSubscription catchupCatchupSubscription)
        {
            _logger = logger;
            _catchupSubscription = catchupCatchupSubscription;
        }

        public List<string> GetAccounts()
        {
            return _sanctionedNames;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return _catchupSubscription.StartAsync(SubscriptionNames.Accounts.AllAccountsOpened, "AccountsCatchupHostedService", cancellationToken,
                (subscription, @event, json, ct) =>
                {
                    _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType}");
                    return @event.Event.EventType switch
                    {
                        nameof(AccountOpenedEvent_v1) => HandleAccountOpenedEvent(@event, json),
                        _ => throw new NotImplementedException()
                    };
                });
        }

        private Task HandleAccountOpenedEvent(ResolvedEvent @event, string json)
        {
            var eventData = JsonSerializer.Deserialize<SanctionedNameAdded_v1>(json);

            if (eventData is null || string.IsNullOrEmpty(eventData.SanctionedName))
            {
                _logger.LogWarning($"eventData is null or missing a sanctioned name to add, ignoring event {SubscriptionNames.Sanctions.GlobalSanctionedNames}#{@event.OriginalEventNumber}");
                return Task.CompletedTask;
            }

            if (_sanctionedNames.All(x => x != eventData.SanctionedName))
                _sanctionedNames.Add(eventData.SanctionedName);

            return Task.CompletedTask;
        }
    }
}