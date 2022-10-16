using System.ComponentModel;
using AJP.MediatrEndpoints.PropertyAttributes;
using MediatR;

namespace payment_scheme_simulator.RequestHandlers;

public class SimulateRandomInboundPaymentRequest : IRequest<SimulatedInboundPaymentResponse>
{
    [OptionalProperty]
    public bool SimulateSanctionedPayment { get; set; }
}