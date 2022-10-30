using System.Collections.Generic;
using Domain.Events.Payments;

namespace sanctions_api.RequestHandlers.HeldPayments;

public class GetHeldPaymentsResponse
{
    public List<HeldPayment> HeldPayments { get; init; }
}