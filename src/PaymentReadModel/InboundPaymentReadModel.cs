﻿using Domain;
using Domain.Interfaces;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Logging;
using PaymentSchemeDomain.Events;

namespace PaymentReadModel;

public class InboundPaymentReadModel : IInboundPaymentReadModel
{
    private readonly ILogger<InboundPaymentReadModel> _logger;
    private readonly IEventStreamReader _eventStreamReader;

    private readonly IEventDeserialiser _eventDeserialiser;

    //private readonly ICatchupSubscription _catchupSubscription;
    private string? _subscriptionFriendlyName;
    private CancellationTokenSource? _cancellationTokenSource;

    public int SortCode { get; private set; }
    public int AccountNumber { get; private set; }
    public Guid PaymentId { get; private set; }
    public Guid CorrelationId { get; private set; }

    public int OriginatingSortCode { get; private set; }
    public int OriginatingAccountNumber { get; private set; }
    public string? OriginatingAccountName { get; private set; }
    public string? PaymentReference { get; private set; }
    public string? DestinationAccountName { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentScheme Scheme { get; private set; }
    public PaymentType Type { get; private set; }
    public DateTime ProcessingDate { get; private set; }

    public bool PaymentValidated { get; private set; }
    public bool PassedSanctionsCheck { get; private set; }
    public bool PassedAccountStatusCheck { get; private set; }
    public bool FundsCleared { get; private set; }
    public Guid ClearedTransactionId { get; private set; }

    public bool PaymentIsHeld { get; private set; }
    public bool PaymentHasBeenHeld { get; private set; }


    public InboundPaymentReadModel(
        ILogger<InboundPaymentReadModel> logger, 
        IEventStreamReader eventStreamReader, 
        IEventDeserialiser eventDeserialiser) //ICatchupSubscription catchupSubscription)
    {
        _logger = logger;
        _eventStreamReader = eventStreamReader;
        _eventDeserialiser = eventDeserialiser;
        //_catchupSubscription = catchupSubscription;
    }

    public async Task Read(PaymentDirection paymentDirection, int sortCode, int accountNumber, Guid paymentId, CancellationToken cancellationToken)
    {
        SortCode = sortCode;
        AccountNumber = accountNumber;

        _subscriptionFriendlyName = $"InboundPaymentReadModel-{SortCode}-{AccountNumber}";

        _cancellationTokenSource = new CancellationTokenSource();

        var events = await _eventStreamReader.ReadForwards(PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AccountPayments(paymentDirection, SortCode, AccountNumber, paymentId), StreamStartPositions.Default, cancellationToken);

        foreach (var eventWrapper in events)
        {
            _logger.LogTrace($"event read from stream #{eventWrapper.EventNumber} {eventWrapper.EventTypeName} on {_subscriptionFriendlyName}");

            try
            {
                dynamic dynamicEvent = _eventDeserialiser.DeserialiseEvent(eventWrapper);
                HandleEvent(dynamicEvent);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException))
                {
                    Console.WriteLine($"Missing handler for {eventWrapper.EventTypeName}, consider adding one if this event's properties are important to this particular aggregate.");
                    return;
                }

                Console.WriteLine(e);
                throw;
            }
        }

        _logger.LogDebug($"Completed reading events from stream {_subscriptionFriendlyName}");

        //await _catchupSubscription.StartAsync(PaymentSchemeDomainStreamNames.Accounts.AccountTransactions(SortCode, AccountNumber, correlationId),
        //    _subscriptionFriendlyName, _cancellationTokenSource.Token, (subscription, @event, json, cancellationToken) =>
        //    {
        //        _logger.LogInformation($"event appeared #{@event.OriginalEventNumber} {@event.Event.EventType} on {_subscriptionFriendlyName}");
        //        dynamic dynamicEvent = _eventDeserialiser.DeserialiseEvent(@event.typeName, @event.json);
        //        HandleEvent(dynamicEvent);
        //    });
    }

    private Task HandleEvent(InboundPaymentReceived_v1 eventData)
    {
        PaymentId = eventData.PaymentId;
        CorrelationId = eventData.CorrelationId;
        OriginatingSortCode = eventData.OriginatingSortCode;
        OriginatingAccountNumber = eventData.OriginatingAccountNumber;
        OriginatingAccountName = eventData.OriginatingAccountName;
        DestinationAccountName = eventData.DestinationAccountName;
        PaymentReference = eventData.PaymentReference;
        Amount = eventData.Amount;
        Scheme = eventData.Scheme;
        Type = eventData.Type;
        ProcessingDate = eventData.ProcessingDate;

        return Task.CompletedTask;
    }

    private Task HandleEvent(InboundPaymentValidated_v1 _)
    {
        PaymentValidated = true;
        return Task.CompletedTask;
    }

    private Task HandleEvent(InboundPaymentSanctionsChecked_v1 _)
    {
        PassedSanctionsCheck = true;
        return Task.CompletedTask;
    }

    private Task HandleEvent(InboundPaymentAccountStatusChecked_v1 _)
    {
        PassedAccountStatusCheck = true;
        return Task.CompletedTask;
    }

    private Task HandleEvent(InboundPaymentBalanceUpdated_v1 eventData)
    {
        ClearedTransactionId = eventData.ClearedTransactionId;
        FundsCleared = true;
        return Task.CompletedTask;
    }

    private Task HandleEvent(InboundPaymentHeld_v1 _)
    {
        PaymentHasBeenHeld = true;
        PaymentIsHeld = true;
        return Task.CompletedTask;
    }
}