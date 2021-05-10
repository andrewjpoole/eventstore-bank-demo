using System.Threading.Tasks;
using EventStore.Client;
//using EventStore.ClientAPI;

namespace infrastructure.EventStore
{
    //public interface IEventStoreConnectionFactory
    //{
    //    Task<IEventStoreConnection> CreateConnectionAsync();
    //}

    public interface IEventStoreClientFactory
    {
        EventStoreClient CreateClient();
    }
}