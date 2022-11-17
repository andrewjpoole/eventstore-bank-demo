using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interfaces;
using Infrastructure.EventStore.Serialisation;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentSchemeDomain.Events;
using SanctionsDomain.ServiceInterfaces;

namespace SanctionsApp.Services;

public class HeldPaymentsCatchupHostedService : BackgroundService, IHeldPaymentsCatchupHostedService
{
    private readonly ILogger<SanctionedNamesCatchupHostedService> _logger;
    private readonly ICatchupSubscription _catchupSubscription;
    private readonly IEventDeserialiser _eventDeserialiser;

    private readonly Dictionary<Guid, HeldPayment> _heldPayments = new();

    public HeldPaymentsCatchupHostedService(
        ILogger<SanctionedNamesCatchupHostedService> logger, 
        ICatchupSubscription catchupCatchupSubscription, 
        IEventDeserialiser eventDeserialiser)
    {
        _logger = logger;
        _catchupSubscription = catchupCatchupSubscription;
        _eventDeserialiser = eventDeserialiser;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _catchupSubscription.StartAsync(PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllPayments, "HeldPaymentsCatchupHostedService", cancellationToken,
            (eventWrapper, ct) =>
            {
                _logger.LogTrace($"event appeared #{eventWrapper.EventNumber} {eventWrapper.EventTypeName}");
                dynamic @event = _eventDeserialiser.DeserialiseEvent(eventWrapper);
                try
                {
                    HandleEvent(@event);
                }
                catch (RuntimeBinderException)
                {
                    // Ignore events that we haven't provided a handler for.
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