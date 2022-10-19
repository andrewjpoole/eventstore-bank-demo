using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Events.Sanctions;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace sanctions_api.Services;

public class SanctionsCatchupHostedService : BackgroundService, ISanctionsCatchupHostedService
{
    private readonly ILogger<SanctionsCatchupHostedService> _logger;
    private readonly ICatchupSubscription _catchupSubscription;
    private readonly IEventDeserialiser _eventDeserialiser;
    private readonly List<string> _sanctionedNames = new();

    public SanctionsCatchupHostedService(ILogger<SanctionsCatchupHostedService> logger, ICatchupSubscription catchupCatchupSubscription, IEventDeserialiser eventDeserialiser)
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
        return _catchupSubscription.StartAsync(StreamNames.Sanctions.GlobalSanctionedNames, "SanctionsCatchupHostedService", cancellationToken,
            (subscription, eventWrapper, ct) =>
            {
                _logger.LogInformation($"event appeared #{eventWrapper.EventNumber} {eventWrapper.EventTypeName}");
                dynamic @event = _eventDeserialiser.DeserialiseEvent(eventWrapper);

                return Task.CompletedTask;
                //return eventWrapper.EventTypeName switch
                //{
                //    nameof(SanctionedNameAdded_v1) => HandleSanctionedNameAdded(@event, json),
                //    nameof(SanctionedNameRemoved_v1) => HandleSanctionedNameRemoved(@event, json),
                //    _ => throw new NotImplementedException()
                //};
            });
    }

    private Task HandleSanctionedNameAdded(SanctionedNameAdded_v1 @event, IEventWrapper eventWrapper)
    {
        if (@event is null || string.IsNullOrEmpty(@event.SanctionedName))
        {
            _logger.LogWarning($"eventData is null or missing a sanctioned name to add, ignoring event {StreamNames.Sanctions.GlobalSanctionedNames}#{eventWrapper.EventNumber}");
            return Task.CompletedTask;
        }

        if (_sanctionedNames.All(x => x != @event.SanctionedName))
            _sanctionedNames.Add(@event.SanctionedName);

        return Task.CompletedTask;
    }

    private Task HandleSanctionedNameRemoved(SanctionedNameRemoved_v1 @event, IEventWrapper eventWrapper)
    {
        if (@event is null || string.IsNullOrEmpty(@event.SanctionedName))
        {
            _logger.LogWarning($"eventData is null or missing a sanctioned name to remove, ignoring event {StreamNames.Sanctions.GlobalSanctionedNames}#{eventWrapper.EventNumber}");
            return Task.CompletedTask;
        }

        if (_sanctionedNames.Contains(@event.SanctionedName))
            _sanctionedNames.Remove(@event.SanctionedName);

        return Task.CompletedTask;
    }
}