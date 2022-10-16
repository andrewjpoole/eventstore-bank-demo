using System.Threading;
using System.Threading.Tasks;
using Domain.Events.Payments;
using Infrastructure.EventStore;
using MediatR;
using payment_scheme_simulator.Services;

namespace payment_scheme_simulator.RequestHandlers;

public class SimulateRandomInboundPaymentRequestHandler : IRequestHandler<SimulateRandomInboundPaymentRequest, SimulatedInboundPaymentResponse>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IRandomInboundPaymentReceivedGenerator _randomInboundPaymentReceivedGenerator;

    public SimulateRandomInboundPaymentRequestHandler(IEventPublisher eventPublisher, IRandomInboundPaymentReceivedGenerator randomInboundPaymentReceivedGenerator)
    {
        _eventPublisher = eventPublisher;
        _randomInboundPaymentReceivedGenerator = randomInboundPaymentReceivedGenerator;
    }

    public async Task<SimulatedInboundPaymentResponse> Handle(SimulateRandomInboundPaymentRequest request, CancellationToken cancellationToken)
    {
        var cancellationTokenSource = new CancellationTokenSource();
            
        var @event = await _randomInboundPaymentReceivedGenerator.Generate(PaymentScheme.Bacs, PaymentType.Credit, request.SimulateSanctionedPayment);

        var result = await _eventPublisher.Publish(@event, @event.StreamName(), cancellationTokenSource.Token);

        return new SimulatedInboundPaymentResponse
        {
            PaymentId = @event.PaymentId,
            CorrelationId = @event.CorrelationId,
            Amount = @event.Amount,
            PaymentReference = @event.PaymentReference,
            OriginatingSortCode = @event.OriginatingSortCode,
            OriginatingAccountNumber = @event.OriginatingAccountNumber,
            OriginatingAccountName = @event.OriginatingAccountName,
            DestinationSortCode = @event.DestinationSortCode,
            DestinationAccountNumber = @event.DestinationAccountNumber,
            DestinationAccountName = @event.DestinationAccountName,
            ProcessingDate = @event.ProcessingDate,
            Scheme = @event.Scheme,
            Type = @event.Type
        };
    }
}