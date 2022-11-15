using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Interfaces;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SanctionsDomain;
using SanctionsDomain.Events;

namespace sanctions_api.Services;

public class SanctionedNamesCatchupHostedService : BackgroundService, ISanctionedNamesCatchupHostedService
{
    private readonly ILogger<SanctionedNamesCatchupHostedService> _logger;
    private readonly ICatchupSubscription _catchupSubscription;
    private readonly IEventDeserialiser _eventDeserialiser;
    private readonly List<string> _sanctionedNames = new();

    public SanctionedNamesCatchupHostedService(ILogger<SanctionedNamesCatchupHostedService> logger, ICatchupSubscription catchupCatchupSubscription, IEventDeserialiser eventDeserialiser)
    {
        _logger = logger;
        _catchupSubscription = catchupCatchupSubscription;
        _eventDeserialiser = eventDeserialiser;
    }

    public List<string> GetSanctionedNames()
    {
        return _sanctionedNames;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _catchupSubscription.StartAsync(SanctionsDomainStreamNames.GlobalSanctionedNames, "SanctionedNamesCatchupHostedService", cancellationToken,
            (eventWrapper, ct) =>
            {
                _logger.LogInformation($"event appeared #{eventWrapper.EventNumber} {eventWrapper.EventTypeName}");
                dynamic @event = _eventDeserialiser.DeserialiseEvent(eventWrapper);
                HandleEvent(@event, eventWrapper);
                return Task.CompletedTask;
            });
    }

    private Task HandleEvent(SanctionedNameAdded_v1 @event, IEventWrapper eventWrapper)
    {
        if (@event is null || string.IsNullOrEmpty(@event.SanctionedName))
        {
            _logger.LogWarning($"eventData is null or missing a sanctioned name to add, ignoring event {SanctionsDomainStreamNames.GlobalSanctionedNames}#{eventWrapper.EventNumber}");
            return Task.CompletedTask;
        }

        if (_sanctionedNames.All(x => x != @event.SanctionedName))
            _sanctionedNames.Add(@event.SanctionedName);

        return Task.CompletedTask;
    }

    private Task HandleEvent(SanctionedNameRemoved_v1 @event, IEventWrapper eventWrapper)
    {
        if (@event is null || string.IsNullOrEmpty(@event.SanctionedName))
        {
            _logger.LogWarning($"eventData is null or missing a sanctioned name to remove, ignoring event {SanctionsDomainStreamNames.GlobalSanctionedNames}#{eventWrapper.EventNumber}");
            return Task.CompletedTask;
        }

        if (_sanctionedNames.Contains(@event.SanctionedName))
            _sanctionedNames.Remove(@event.SanctionedName);

        return Task.CompletedTask;
    }
}