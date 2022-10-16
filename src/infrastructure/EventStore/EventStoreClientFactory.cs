using EventStore.Client;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.EventStore;

public class EventStoreClientFactory : IEventStoreClientFactory
{
    private readonly string _eventStoreConnectionString;
    private const string EventStoreConnectionStringConfigKey = "EventStoreConnectionsString";

    public EventStoreClientFactory(IConfiguration configuration)
    {
        _eventStoreConnectionString = configuration[EventStoreConnectionStringConfigKey];
    }

    public EventStoreClient CreateClient()
    {
        var settings = EventStoreClientSettings
            .Create(_eventStoreConnectionString);
        var client = new EventStoreClient(settings);
        return client;
    }
}