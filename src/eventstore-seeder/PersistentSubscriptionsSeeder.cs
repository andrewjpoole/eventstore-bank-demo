using EventStore.Client;

namespace eventstore_seeder
{
    public class PersistentSubscriptionsSeeder
    {
        public void Seed()
        {
            var persistentSubscriptionFactory = new PersistentSubscriptionFactory();
            var settings = new PersistentSubscriptionSettings(
                resolveLinkTos: true,
                startFrom: StreamPosition.Start);

            persistentSubscriptionFactory.Create("$et-InboundPaymentReceived_v1 ", "$et-InboundPaymentReceived_v1-SubscriptionGroup", settings);
        }
    }
}