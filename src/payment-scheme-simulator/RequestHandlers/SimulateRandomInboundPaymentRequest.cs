using System.Collections.Generic;
using System.Linq;
using MediatR;

namespace payment_scheme_simulator.RequestHandlers
{
    public class SimulateRandomInboundPaymentRequest : IRequest<SimulatedInboundPaymentResponse>
    {
    }
}
