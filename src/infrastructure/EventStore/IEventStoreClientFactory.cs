using EventStore.Client;

namespace infrastructure.EventStore
{
    public interface IEventStoreClientFactory
    {
        EventStoreClient CreateClient();
    }
}