using System;
using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IEventSubscription<T> where T : IEvent
{
    Task SubscribeToStream(string streamName, Action<T, IEventWrapper> handleEventAppeared);
}