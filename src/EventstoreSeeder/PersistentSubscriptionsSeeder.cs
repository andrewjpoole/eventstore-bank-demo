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
            
        persistentSubscriptionFactory.Create(StreamNames.PaymentProcessing.AllInboundPaymentReceived, StreamNames.SubscriptionGroupName(StreamNames.PaymentProcessing.AllInboundPaymentReceived), settings);
        persistentSubscriptionFactory.Create(StreamNames.PaymentProcessing.AllInboundPaymentValidated, StreamNames.SubscriptionGroupName(StreamNames.PaymentProcessing.AllInboundPaymentValidated), settings);
        persistentSubscriptionFactory.Create(StreamNames.PaymentProcessing.AllInboundPaymentSanctionsChecked, StreamNames.SubscriptionGroupName(StreamNames.PaymentProcessing.AllInboundPaymentSanctionsChecked), settings);
        persistentSubscriptionFactory.Create(StreamNames.PaymentProcessing.AllInboundPaymentAccountStatusChecked, StreamNames.SubscriptionGroupName(StreamNames.PaymentProcessing.AllInboundPaymentAccountStatusChecked), settings);
        persistentSubscriptionFactory.Create(StreamNames.PaymentProcessing.AllInboundPaymentBalanceUpdated, StreamNames.SubscriptionGroupName(StreamNames.PaymentProcessing.AllInboundPaymentBalanceUpdated), settings);
    }
}