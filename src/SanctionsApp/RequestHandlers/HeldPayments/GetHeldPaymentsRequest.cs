using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AJP.MediatrEndpoints.Exceptions;
using AJP.MediatrEndpoints.PropertyAttributes;
using Domain.Events.Payments;
using Infrastructure.EventStore;
using Microsoft.AspNetCore.Http;
using OneOf;
using OneOf.Types;
using sanctions_api.Services;

namespace sanctions_api.RequestHandlers.HeldPayments
{
    public class GetHeldPaymentsRequest : IRequest<GetHeldPaymentsResponse>
    {
        [OptionalProperty]
        public bool ExcludeReleasedPayments { get; set; }
    }

    public class GetHeldPaymentsResponse
    {
        public List<HeldPayment> HeldPayments { get; init; }
    }
    
    public class GetHeldPaymentsRequestHandler : IRequestHandler<GetHeldPaymentsRequest, GetHeldPaymentsResponse>
    {
        private readonly IHeldPaymentsCatchupHostedService _heldPaymentsCatchupHostedService;

        public GetHeldPaymentsRequestHandler(IHeldPaymentsCatchupHostedService heldPaymentsCatchupHostedService)
        {
            _heldPaymentsCatchupHostedService = heldPaymentsCatchupHostedService;
        }

        public Task<GetHeldPaymentsResponse> Handle(GetHeldPaymentsRequest request, CancellationToken cancellationToken)
        {
            var response = new GetHeldPaymentsResponse
            {
                HeldPayments = _heldPaymentsCatchupHostedService.GetHeldPayments(request.ExcludeReleasedPayments)
            };
            return Task.FromResult(response);
        }
    }

    public class ReleaseHeldPaymentRequest : IRequest<ReleaseHeldPaymentResponse>
    {
        public string PaymentId { get; init; }
        public string ReleasedBy { get; init; }
        public string ReleasedReason { get; init; }
        public Guid HeldPaymentReleaseToken { get; init; }

        public OneOf<True, List<string>> IsValid()
        {
            var validationErrors = new List<string>();
            if(string.IsNullOrEmpty(PaymentId))
                validationErrors.Add("PaymentId is required");

            if (string.IsNullOrEmpty(ReleasedBy))
                validationErrors.Add("ReleasedBy is required");

            if (string.IsNullOrEmpty(ReleasedReason))
                validationErrors.Add("ReleasedReason is required");

            return validationErrors.Any() ? validationErrors : new True();
        }
    }

    public class ReleaseHeldPaymentResponse
    {
        public string PaymentId { get; init; }
        public string ReleasedBy { get; init; }
        public DateTime ReleasedAt { get; init; }
    }
    

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
}
