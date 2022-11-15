using System;
using System.Threading;
using System.Threading.Tasks;
using AccountsDomain;
using AccountsDomain.Events;
using Domain;
using Domain.Interfaces;
using Infrastructure.EventStore.Serialisation;
using Microsoft.Extensions.Logging;

namespace PaymentSchemeApp.Services;

public class AccountDetailsReadModel : IAccountDetailsReadModel
{
    private readonly ILogger<AccountDetailsReadModel> _logger;
    private readonly IEventStreamReader _eventStreamReader;
    private readonly IEventDeserialiser _eventDeserialiser;
    private string? _subscriptionFriendlyName;
        
    public int SortCode { get; private set; }
    public int AccountNumber { get; private set; }
    public string? Name { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime Opened { get; private set; }

    public AccountDetailsReadModel(ILogger<AccountDetailsReadModel> logger, IEventStreamReader eventStreamReader, IEventDeserialiser eventDeserialiser)
    {
        _logger = logger;
        _eventStreamReader = eventStreamReader;
        _eventDeserialiser = eventDeserialiser;
    }

    public async Task Read(int sortCode, int accountNumber, CancellationToken cancellationToken)
    {
        SortCode = sortCode;
        AccountNumber = accountNumber;

        _subscriptionFriendlyName = $"AccountDetailsReadModel-{SortCode}-{AccountNumber}";

        var events = await _eventStreamReader.ReadForwards(AccountsDomainStreamNames.AccountDetails(SortCode, AccountNumber), StreamStartPositions.Default, cancellationToken);

        foreach (var eventWrapper in events)
        {
            _logger.LogDebug($"event read from stream #{eventWrapper.EventNumber} {eventWrapper.EventTypeName} on {_subscriptionFriendlyName}");
            try
            {
                dynamic dynamicEvent = _eventDeserialiser.DeserialiseEvent(eventWrapper);
                HandleEvent(dynamicEvent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private Task HandleEvent(AccountOpenedEvent_v1 eventData)
    {
        Name = eventData.Name;
        Status = eventData.Status;
        Opened = eventData.Opened;

        return Task.CompletedTask;
    }

    private Task HandleEvent(AccountStatusUpdated_v1 eventData)
    {
        Status = eventData.Status;

        return Task.CompletedTask;
    }
}