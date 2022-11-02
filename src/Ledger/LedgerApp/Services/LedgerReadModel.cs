using Domain;
using Domain.Interfaces;
using Infrastructure.EventStore.Serialisation;
using LedgerDomain.Events;
using LedgerDomain.ReadModel;

namespace LedgerApp.Services;

public class LedgerReadModel : ILedgerReadModel
{
    private readonly ILogger<LedgerReadModel> _logger;
    private readonly IEventDeserialiser _eventDeserialiser;
    private readonly IEventStreamReader _eventStreamReader;
    private string? _subscriptionFriendlyName;

    private decimal _balance;

    public int SortCode { get; private set; }
    public int AccountNumber { get; private set; }
    public decimal Balance { get; private set; }

    public LedgerReadModel(ILogger<LedgerReadModel> logger, IEventDeserialiser eventDeserialiser, IEventStreamReader eventStreamReader)
    {
        _logger = logger;
        _eventDeserialiser = eventDeserialiser;
        _eventStreamReader = eventStreamReader;
    }

    public async Task Read(int sortCode, int accountNumber, CancellationToken cancellationToken)
    {
        SortCode = sortCode;
        AccountNumber = accountNumber;
        _balance = 0;

        _subscriptionFriendlyName = $"InboundPaymentReadModel-{SortCode}-{AccountNumber}";

        // Todo possibly first look up flags/overdraft limit etc from the account?
        
        var events = await _eventStreamReader.ReadForwards(StreamNames.Ledger.AccountLedger(SortCode, AccountNumber), StreamStartPositions.Default, cancellationToken);

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
                Console.WriteLine(e);
                throw;
            }
        }

        _logger.LogDebug($"Completed reading events from stream {_subscriptionFriendlyName}");
    }

    private Task HandleEvent(LedgerEntryPosted_v1 eventData)
    {
        // update balance
        _balance += eventData.Amount;

        return Task.CompletedTask;
    }

    public decimal CurrentBalance() => _balance;
}