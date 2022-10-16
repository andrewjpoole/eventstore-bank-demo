using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Events.Payments;
using Infrastructure.EventStore;
using MediatR;
using payment_scheme_simulator.Services;

namespace payment_scheme_simulator.RequestHandlers;

public class SimulateInboundPaymentRequestHandler : IRequestHandler<SimulateInboundPaymentRequest, SimulatedInboundPaymentResponse>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IRandomInboundPaymentReceivedGenerator _randomInboundPaymentReceivedGenerator;

    public SimulateInboundPaymentRequestHandler(IEventPublisher eventPublisher, IRandomInboundPaymentReceivedGenerator randomInboundPaymentReceivedGenerator)
    {
        _eventPublisher = eventPublisher;
        _randomInboundPaymentReceivedGenerator = randomInboundPaymentReceivedGenerator;
    }

    public async Task<SimulatedInboundPaymentResponse> Handle(SimulateInboundPaymentRequest request, CancellationToken cancellationToken)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        // generate a random event and then override with specified fields
        var randomEvent = await _randomInboundPaymentReceivedGenerator.Generate(PaymentScheme.Bacs, PaymentType.Credit);

        var @event = new InboundPaymentReceived_v1
        {
            CorrelationId = new Guid(),
            PaymentId = request.PaymentId ?? $"SimulatedManualPayment{Guid.NewGuid()}",
            Amount = request.Amount != 0 ? request.Amount : randomEvent.Amount,
            OriginatingAccountName = request.OriginatingAccountName ?? randomEvent.OriginatingAccountName,
            OriginatingAccountNumber = request.OriginatingAccountNumber != 0 ? request.OriginatingAccountNumber : randomEvent.OriginatingAccountNumber,
            OriginatingSortCode = request.OriginatingSortCode != 0 ? request.OriginatingSortCode : randomEvent.OriginatingSortCode,
            DestinationAccountName = request.DestinationAccountName ?? randomEvent.DestinationAccountName,
            DestinationAccountNumber = request.DestinationAccountNumber != 0 ? request.DestinationAccountNumber : randomEvent.DestinationAccountNumber,
            DestinationSortCode = request.DestinationSortCode != 0 ? request.DestinationSortCode : randomEvent.DestinationSortCode,
            ProcessingDate = DateTime.Now.Date,
            PaymentReference = request.PaymentReference ?? randomEvent.PaymentReference,
            Scheme = PaymentScheme.Bacs,
            Type = PaymentType.Credit,
                
        };

        var result = await _eventPublisher.Publish(@event, @event.StreamName(), cancellationTokenSource.Token);

        return new SimulatedInboundPaymentResponse
        {
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