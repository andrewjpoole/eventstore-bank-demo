using Domain;
using EventStore.Client;

namespace eventstore_seeder;

public class PersistentSubscriptionsSeeder
{
    public void Seed()
    {
        var persistentSubscriptionFactory = new PersistentSubscriptionFactory();
        var settings = new PersistentSubscriptionSettings(
            resolveLinkTos: true,
            startFrom: StreamPosition.Start);
            
        persistentSubscriptionFactory.Create(StreamNames.Payments.AllInboundPaymentReceived, settings);
        persistentSubscriptionFactory.Create(StreamNames.Payments.AllInboundPaymentValidated, settings);
        persistentSubscriptionFactory.Create(StreamNames.Payments.AllInboundPaymentSanctionsChecked, settings);
        persistentSubscriptionFactory.Create(StreamNames.Payments.AllInboundHeldPaymentReleased, settings);
        persistentSubscriptionFactory.Create(StreamNames.Payments.AllInboundPaymentAccountStatusChecked, settings);
        persistentSubscriptionFactory.Create(StreamNames.Payments.AllInboundPaymentBalanceUpdated, settings);
    }
}