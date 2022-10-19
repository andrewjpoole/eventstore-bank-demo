//using System;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Domain;
//using Infrastructure.EventStore.Serialisation;

//namespace Infrastructure.EventStore;

//public class EventSubscription<T> : IEventSubscription<T> where T : IEvent
//{
//    private readonly IEventStoreClientFactory _eventStoreClientFactory;

//    public EventSubscription(IEventStoreClientFactory eventStoreClientFactory)
//    {
//        _eventStoreClientFactory = eventStoreClientFactory;
//    }

//    public async Task SubscribeToStream(string streamName, Action<T, IEventWrapper> handleEventAppeared)
//    {
//        var client = _eventStoreClientFactory.CreateClient();

//        await client.SubscribeToStreamAsync(streamName, (subscription, @event, cancellationToken) =>
//        {
//            var eventData = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(@event.Event.Data.ToArray()));
//            handleEventAppeared(eventData, new EventWrapper(@event));
//            return Task.CompletedTask;
//        });
//    }
//}