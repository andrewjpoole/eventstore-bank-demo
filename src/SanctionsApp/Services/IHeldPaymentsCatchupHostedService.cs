using System.Collections.Generic;
using Domain.Events.Payments;

namespace sanctions_api.Services;

public interface IHeldPaymentsCatchupHostedService
{
    List<HeldPayment> GetHeldPayments(bool excludeReleased = true);
}