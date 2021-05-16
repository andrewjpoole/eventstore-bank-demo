using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using events;
using events.Sanctions;
using EventStore.Client;
using infrastructure.EventStore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace sanctions_api.Services
{
    public class SanctionsCatchupHostedService : BackgroundService, ISanctionsCatchupHostedService
    {
        private readonly ILogger<SanctionsCatchupHostedService> _logger;
        private readonly ICatchupSubscription _catchupSubscription;
        private readonly List<string> _sanctionedNames = new();

        public SanctionsCatchupHostedService(ILogger<SanctionsCatchupHostedService> logger, ICatchupSubscription catchupCatchupSubscription)
        {
            _logger = logger;
            _catchupSubscription = catchupCatchupSubscription;
        }

        public List<string> GetSanctionedNames()
        {
            return _sanctionedNames;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return _catchupSubscription.StartAsync(StreamNames.Sanctions.GlobalSanctionedNames, "SanctionsCatchupHostedService", cancellationToken,
                (subscription, @event, json, ct) =>
                {
                    _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType}");
                    return @event.Event.EventType switch
                    {
                        nameof(SanctionedNameAdded_v1) => HandleSanctionedNameAdded(@event, json),
                        nameof(SanctionedNameRemoved_v1) => HandleSanctionedNameRemoved(@event, json),
                        _ => throw new NotImplementedException()
                    };
                });
        }

        private Task HandleSanctionedNameAdded(ResolvedEvent @event, string json)
        {
            var eventData = JsonSerializer.Deserialize<SanctionedNameAdded_v1>(json);

            if (eventData is null || string.IsNullOrEmpty(eventData.SanctionedName))
            {
                _logger.LogWarning($"eventData is null or missing a sanctioned name to add, ignoring event {StreamNames.Sanctions.GlobalSanctionedNames}#{@event.OriginalEventNumber}");
                return Task.CompletedTask;
            }

            if (_sanctionedNames.All(x => x != eventData.SanctionedName))
                _sanctionedNames.Add(eventData.SanctionedName);

            return Task.CompletedTask;
        }

        private Task HandleSanctionedNameRemoved(ResolvedEvent @event, string json)
        {
            var eventData = JsonSerializer.Deserialize<SanctionedNameRemoved_v1>(json);

            if (eventData is null || string.IsNullOrEmpty(eventData.SanctionedName))
            {
                _logger.LogWarning($"eventData is null or missing a sanctioned name to remove, ignoring event {StreamNames.Sanctions.GlobalSanctionedNames}#{@event.OriginalEventNumber}");
                return Task.CompletedTask;
            }

            if (_sanctionedNames.Contains(eventData.SanctionedName))
                _sanctionedNames.Remove(eventData.SanctionedName);

            return Task.CompletedTask;
        }
    }
}