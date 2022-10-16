using System;
using MediatR;

namespace payment_scheme_simulator.RequestHandlers;

public class SimulateInboundPaymentRequest : IRequest<SimulatedInboundPaymentResponse>
{
    public string PaymentId { get; init; }
    public int OriginatingSortCode { get; init; }
    public int OriginatingAccountNumber { get; init; }
    public string OriginatingAccountName { get; set; }
    public string PaymentReference { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public string DestinationAccountName { get; set; }
    public decimal Amount { get; init; }
}