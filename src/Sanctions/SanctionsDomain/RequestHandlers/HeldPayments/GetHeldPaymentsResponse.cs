using PaymentSchemeDomain.Events;

namespace SanctionsDomain.RequestHandlers.HeldPayments;

public class GetHeldPaymentsResponse
{
    public List<HeldPayment> HeldPayments { get; init; }
}