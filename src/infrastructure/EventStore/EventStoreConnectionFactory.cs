using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;

namespace infrastructure.EventStore
{
    public class EventStoreConnectionFactory : IEventStoreConnectionFactory
    {
        private readonly string _eventStoreConnectionString;
        private const string EventStoreConnectionStringConfigKey = "EventStoreConnectionsString";

        private ConnectionSettings _settings;

        public EventStoreConnectionFactory(IConfiguration configuration)
        {
            _eventStoreConnectionString = configuration[EventStoreConnectionStringConfigKey];

            _settings = ConnectionSettings.Create()
                .DisableTls()
                .Build();
        }

        public async Task<IEventStoreConnection> CreateConnectionAsync()
        {
            var connection = EventStoreConnection.Create(_settings, new Uri(_eventStoreConnectionString));
            await connection.ConnectAsync();
            return connection;
        }
    }
}
