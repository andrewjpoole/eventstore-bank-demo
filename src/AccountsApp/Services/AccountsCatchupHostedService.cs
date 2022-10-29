using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;
using Domain;
using Domain.Events.Accounts;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace accounts_api.Services;

public class AccountsCatchupHostedService : BackgroundService, IAccountsCatchupHostedService
{
    private readonly ILogger<AccountsCatchupHostedService> _logger;
    private readonly ICatchupSubscription _catchupSubscription;
    private readonly IEventDeserialiser _eventDeserialiser;
    private readonly Dictionary<Guid,AccountSummary> _accounts = new();

    public AccountsCatchupHostedService(ILogger<AccountsCatchupHostedService> logger, ICatchupSubscription catchupCatchupSubscription, IEventDeserialiser eventDeserialiser)
    {
        _logger = logger;
        _catchupSubscription = catchupCatchupSubscription;
        _eventDeserialiser = eventDeserialiser;
    }

    public Dictionary<Guid, AccountSummary> Accounts => _accounts;

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _catchupSubscription.StartAsync(StreamNames.Accounts.AllAccounts, "AccountsCatchupHostedService", cancellationToken,
            (subscription, eventWrapper, ct) =>
            {
                _logger.LogTrace($"event appeared #{eventWrapper.EventNumber} {eventWrapper.EventTypeName}");
                try
                {
                    dynamic dynamicEvent = _eventDeserialiser.DeserialiseEvent(eventWrapper);
                    HandleEvent(dynamicEvent, eventWrapper);
                }
                catch (RuntimeBinderException)
                {
                    // Ignore events that we haven't provided a handler for.
                }
                return Task.CompletedTask;
            });
    }

    private void HandleEvent(AccountOpenedEvent_v1 @event, IEventWrapper eventWrapper)
    {
        if (@event is null || @event.Id == Guid.Empty)
        {
            _logger.LogWarning($"eventData is null or missing or malformed, ignoring event {StreamNames.Sanctions.GlobalSanctionedNames}#{eventWrapper.EventNumber}");
        }

        if (!_accounts.ContainsKey(@event.Id))
            _accounts.Add(@event.Id, new AccountSummary
            {
                Id = @event.Id,
                SortCode = @event.SortCode,
                AccountNumber = @event.AccountNumber,
                AccountName = @event.Name,
                Status = @event.Status,
                Opened = @event.Opened
            });
    }

    private void HandleEvent(AccountStatusUpdated_v1 @event, IEventWrapper eventWrapper)
    {
        if (@event is null || @event.Id == Guid.Empty)
        {
            _logger.LogWarning($"eventData is null or missing a sanctioned name to add, ignoring event {StreamNames.Sanctions.GlobalSanctionedNames}#{eventWrapper.EventNumber}");
        }

        var account = _accounts[@event.Id];
        if (account is null)
        {
            //log
            //?
        }

        //var accountWithNewStatus = account.WithStatus(eventData.Status);
        var accountWithNewStatus = account.With(a => a.Status, @event.Status);
        _accounts[@event.Id] = accountWithNewStatus;
    }
}