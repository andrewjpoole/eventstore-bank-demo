using System.Collections.Generic;
using PaymentSchemeDomain.Events;

namespace sanctions_api.RequestHandlers.HeldPayments;

public class GetHeldPaymentsResponse
{
    public List<HeldPayment> HeldPayments { get; init; }
}