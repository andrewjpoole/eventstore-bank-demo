using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Events.Payments;
using Infrastructure.EventStore;
using Infrastructure.EventStore.Serialisation;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace sanctions_api.Services;

public class HeldPaymentsCatchupHostedService : BackgroundService, IHeldPaymentsCatchupHostedService
{
    private readonly ILogger<SanctionsCatchupHostedService> _logger;
    private readonly ICatchupSubscription _catchupSubscription;
    private readonly IEventDeserialiser _eventDeserialiser;

    private readonly Dictionary<string, HeldPayment> _heldPayments = new();

    public HeldPaymentsCatchupHostedService(ILogger<SanctionsCatchupHostedService> logger, ICatchupSubscription catchupCatchupSubscription, IEventDeserialiser eventDeserialiser)
    {
        _logger = logger;
        _catchupSubscription = catchupCatchupSubscription;
        _eventDeserialiser = eventDeserialiser;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _catchupSubscription.StartAsync(StreamNames.Accounts.AllAccounts, "HeldPaymentsCatchupHostedService", cancellationToken,
            (subscription, @event, json, ct) =>
            {
                _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType}");
                dynamic typedEvent = _eventDeserialiser.DeserialiseEvent(@event.Event.EventType, json);
                try
                {
                    HandleEvent(typedEvent);
                }
                catch (RuntimeBinderException e)
                {
                }
                return Task.CompletedTask;

            });
    }

    public void HandleEvent(InboundPaymentHeld_v1 @event)
    {
        var heldPayment = new HeldPayment
        {
            PaymentId = @event.PaymentId,
            Reason = @event.Reason,
            CorrelationId = @event.CorrelationId,
            DestinationSortCode = @event.DestinationSortCode,
            DestinationAccountNumber = @event.DestinationAccountNumber,
            DestinationAccountName = @event.DestinationAccountName,
            OriginatingSortCode = @event.OriginatingSortCode,
            OriginatingAccountNumber = @event.OriginatingAccountNumber,
            OriginatingAccountName = @event.OriginatingAccountName,
            Amount = @event.Amount,
            PaymentReference = @event.PaymentReference,
            ProcessingDate = @event.ProcessingDate,
            Scheme = @event.Scheme,
            Type = @event.Type,
            HeldPaymentReleaseToken = Guid.NewGuid()
        };
        
        if(_heldPayments.ContainsKey(@event.PaymentId))
            _heldPayments[@event.PaymentId] = heldPayment;
        else
            _heldPayments.Add(@event.PaymentId, heldPayment);
    }

    public void HandleEvent(InboundHeldPaymentReleased_v1 @event)
    {
        if (_heldPayments.ContainsKey(@event.PaymentId))
            _heldPayments[@event.PaymentId].Release(@event);
        else
            throw new ApplicationException($"Dictionary does not contain a HeldPayment for PaymentId: {@event.PaymentId}");
    }
    
    public List<HeldPayment> GetHeldPayments(bool excludeReleased = true)
    {
        return excludeReleased ? _heldPayments.Values.Where(x => x.IsReleased == false).ToList() : _heldPayments.Values.ToList();
    }
}