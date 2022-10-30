using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.Exceptions;
using Domain.Events.Payments;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using sanctions_api.Services;

namespace sanctions_api.RequestHandlers.HeldPayments;

public class ReleaseHeldPaymentRequestHandler : IRequestHandler<ReleaseHeldPaymentRequest, ReleaseHeldPaymentResponse>
{
    private readonly IHeldPaymentsCatchupHostedService _heldPaymentsCatchupHostedService;
    private readonly IEventPublisher _eventPublisher;

    public ReleaseHeldPaymentRequestHandler(IHeldPaymentsCatchupHostedService heldPaymentsCatchupHostedService, IEventPublisher eventPublisher)
    {
        _heldPaymentsCatchupHostedService = heldPaymentsCatchupHostedService;
        _eventPublisher = eventPublisher;
    }

    public Task<ReleaseHeldPaymentResponse> Handle(ReleaseHeldPaymentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = request.IsValid();
        if (validationResult.IsT1)
            throw new BadHttpRequestException($"Bad Request: {string.Join(", ", validationResult.AsT1)}");

        var heldPayments = _heldPaymentsCatchupHostedService.GetHeldPayments();
        var foundHeldPayment = heldPayments.FirstOrDefault(p =>
                                   p.PaymentId == request.PaymentId && p.HeldPaymentReleaseToken == request.HeldPaymentReleaseToken)
                               ?? throw new NotFoundHttpException($"Could not find the specified held payment");
            
        var releasedEvent = new InboundHeldPaymentReleased_v1
        {
            PaymentId = request.PaymentId,
            ReleasedAt = DateTime.Now,
            ReleasedBy = request.ReleasedBy,
            ReleasedReason = request.ReleasedReason,
            CorrelationId = foundHeldPayment.CorrelationId,
            DestinationSortCode = foundHeldPayment.DestinationSortCode,
            DestinationAccountNumber = foundHeldPayment.DestinationAccountNumber
        };

        _eventPublisher.Publish(releasedEvent, releasedEvent.StreamName(), cancellationToken); // ToDo use StreamRevision? pass through HeldPayment?

        return Task.FromResult(new ReleaseHeldPaymentResponse());

    }
}