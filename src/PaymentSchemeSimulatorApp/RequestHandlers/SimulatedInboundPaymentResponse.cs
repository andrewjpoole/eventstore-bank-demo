using System;
using Domain.Events.Payments;

namespace payment_scheme_simulator.RequestHandlers;

public class SimulatedInboundPaymentResponse
{
    public Guid PaymentId { get; init; }
    public Guid CorrelationId { get; init; }
    public int OriginatingSortCode { get; init; }
    public int OriginatingAccountNumber { get; init; }
    public string OriginatingAccountName { get; set; }
    public string PaymentReference { get; init; }
    public int DestinationSortCode { get; init; }
    public int DestinationAccountNumber { get; init; }
    public string DestinationAccountName { get; set; }
    public decimal Amount { get; init; }
    public PaymentScheme Scheme { get; init; }
    public PaymentType Type { get; init; }
    public DateTime ProcessingDate { get; init; }
}