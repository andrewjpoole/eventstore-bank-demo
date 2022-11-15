using System.Collections.Generic;
using PaymentSchemeDomain.Events;

namespace sanctions_api.Services;

public interface IHeldPaymentsCatchupHostedService
{
    List<HeldPayment> GetHeldPayments(bool excludeReleased = true);
}