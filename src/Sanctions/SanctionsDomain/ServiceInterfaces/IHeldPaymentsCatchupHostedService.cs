using PaymentSchemeDomain.Events;

namespace SanctionsDomain.ServiceInterfaces;

public interface IHeldPaymentsCatchupHostedService
{
    List<HeldPayment> GetHeldPayments(bool excludeReleased = true);
}