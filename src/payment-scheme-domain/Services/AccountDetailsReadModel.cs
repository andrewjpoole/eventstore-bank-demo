using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using events;
using events.Accounts;
using events.Payments;
using EventStore.Client;
using infrastructure.EventStore;
using Microsoft.Extensions.Logging;

namespace payment_scheme_domain.Services
{
    public class AccountDetailsReadModel : IAccountDetailsReadModel
    {
        private readonly ILogger<AccountDetailsReadModel> _logger;
        private readonly IEventStreamReader _eventStreamReader;
        private string _subscriptionFriendlyName;
        
        public int SortCode { get; private set; }
        public int AccountNumber { get; private set; }
        public string Name { get; private set; }
        public AccountStatus Status { get; private set; }
        public DateTime Opened { get; private set; }

        public AccountDetailsReadModel(ILogger<AccountDetailsReadModel> logger, IEventStreamReader eventStreamReader)
        {
            _logger = logger;
            _eventStreamReader = eventStreamReader;
        }

        public async Task Read(int sortCode, int accountNumber, CancellationToken cancellationToken)
        {
            SortCode = sortCode;
            AccountNumber = accountNumber;

            _subscriptionFriendlyName = $"AccountDetailsReadModel-{SortCode}-{AccountNumber}";

            var events = await _eventStreamReader.Read(
                StreamNames.Accounts.AccountDetails(SortCode, AccountNumber), Direction.Forwards, StreamPosition.Start, cancellationToken);

            foreach (var @event in events)
            {
                _logger.LogDebug($"event read from stream #{@event.EventMetadata.EventNumber} {@event.typeName} on {_subscriptionFriendlyName}");
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                _ = @event.typeName switch
                {
                    nameof(AccountOpenedEvent_v1) => HandleEvent(JsonSerializer.Deserialize<AccountOpenedEvent_v1>(@event.json, options)),
                    nameof(AccountStatusUpdated_v1) => HandleEvent(JsonSerializer.Deserialize<AccountStatusUpdated_v1>(@event.json, options)),
                    _ => throw new NotImplementedException()
                };
            }
        }

        private Task HandleEvent(AccountOpenedEvent_v1? eventData)
        {
            Name = eventData.Name;
            Status = eventData.Status;
            Opened = eventData.Opened;

            return Task.CompletedTask;
        }

        private Task HandleEvent(AccountStatusUpdated_v1? eventData)
        {
            Status = eventData.Status;

            return Task.CompletedTask;
        }
    }
}