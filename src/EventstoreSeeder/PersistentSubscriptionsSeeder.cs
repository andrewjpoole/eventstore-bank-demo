using Domain;
using EventStore.Client;

namespace EventstoreSeeder;

public class PersistentSubscriptionsSeeder
{
    public void Seed()
    {
        var persistentSubscriptionFactory = new PersistentSubscriptionFactory();
        var settings = new PersistentSubscriptionSettings(
            resolveLinkTos: true,
            startFrom: StreamPosition.Start);
            
        persistentSubscriptionFactory.Create(PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllInboundPaymentReceived, settings);
        persistentSubscriptionFactory.Create(PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllInboundPaymentValidated, settings);
        persistentSubscriptionFactory.Create(PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllInboundPaymentSanctionsChecked, settings);
        persistentSubscriptionFactory.Create(PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllInboundHeldPaymentReleased, settings);
        persistentSubscriptionFactory.Create(PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllInboundPaymentAccountStatusChecked, settings);
        persistentSubscriptionFactory.Create(PaymentSchemeDomain.PaymentSchemeDomainStreamNames.AllInboundPaymentBalanceUpdated, settings);
    }
}