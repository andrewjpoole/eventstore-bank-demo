using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace infrastructure.EventStore
{
    public interface IEventStoreConnectionFactory
    {
        Task<IEventStoreConnection> CreateConnectionAsync();
    }
}