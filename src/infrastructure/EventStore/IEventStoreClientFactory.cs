using EventStore.Client;

namespace Infrastructure.EventStore;

public interface IEventStoreClientFactory
{
    EventStoreClient CreateClient();
}