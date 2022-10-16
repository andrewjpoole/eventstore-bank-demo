using EventStore.Client;

//using EventStore.ClientAPI;

namespace Infrastructure.EventStore;
//public interface IEventStoreConnectionFactory
//{
//    Task<IEventStoreConnection> CreateConnectionAsync();
//}

public interface IEventStoreClientFactory
{
    EventStoreClient CreateClient();
}