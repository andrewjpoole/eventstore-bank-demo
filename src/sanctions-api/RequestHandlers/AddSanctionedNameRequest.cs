using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using events.Sanctions;
using infrastructure.EventStore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using sanctions_api.Services;

namespace sanctions_api.RequestHandlers
{
    public class AddSanctionedNameRequest : IRequest<SanctionedNameChangeResponse>
    {
        public string Name { get; init; }
    }

    public class RemoveSanctionedNameRequest : IRequest<SanctionedNameChangeResponse>
    {
        public string Name { get; init; }
    }

    public class SanctionedNameChangeResponse
    {
        public string Message { get; init; }
    }

    public class AddSanctionedNameRequestHandler : IRequestHandler<AddSanctionedNameRequest, SanctionedNameChangeResponse>
    {
        private readonly IEventPublisher _eventPublisher;

        public AddSanctionedNameRequestHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task<SanctionedNameChangeResponse> Handle(AddSanctionedNameRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new BadHttpRequestException("Name must not be empty string");

            var addSanctionedNameEvent = new SanctionedNameAdded_V1
            {
                SanctionedName = request.Name,
                Added = DateTime.Now
            };

            var success = await _eventPublisher.Publish(addSanctionedNameEvent, addSanctionedNameEvent.StreamName(),
                CancellationToken.None);

            if (!success)
                throw new ApplicationException("Couldn't append event, please try again");

            return new SanctionedNameChangeResponse
            {
                Message = $"Sanctioned name {request.Name} added."
            };
        }
    }

    public class RemoveSanctionedNameRequestHandler : IRequestHandler<RemoveSanctionedNameRequest, SanctionedNameChangeResponse>
    {
        private readonly IEventPublisher _eventPublisher;

        public RemoveSanctionedNameRequestHandler(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public async Task<SanctionedNameChangeResponse> Handle(RemoveSanctionedNameRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new BadHttpRequestException("Name must not be empty string");

            var removeSanctionedNameEvent = new SanctionedNameRemoved_V1()
            {
                SanctionedName = request.Name,
                Removed = DateTime.Now
            };

            var success = await _eventPublisher.Publish(removeSanctionedNameEvent, removeSanctionedNameEvent.StreamName(),
                CancellationToken.None);

            if (!success)
                throw new ApplicationException("Couldn't append event, please try again");

            return new SanctionedNameChangeResponse
            {
                Message = $"Sanctioned name {request.Name} removed."
            };
        }
    }

    public class GetCurrentSanctionedNamesRequest : IRequest<CurrentSanctionedNamesResponse>
    {
    }

    public class CurrentSanctionedNamesResponse
    {
        public List<string> SanctionedNames { get; init; }
    }

    public class GetCurrentSanctionedNamesRequestHandler : IRequestHandler<GetCurrentSanctionedNamesRequest, CurrentSanctionedNamesResponse>
    {
        private readonly ISanctionsCatchupHostedService _sanctionedNamesSubscriptionHostedService;

        public GetCurrentSanctionedNamesRequestHandler(ISanctionsCatchupHostedService sanctionedNamesSubscriptionHostedService)
        {
            _sanctionedNamesSubscriptionHostedService = sanctionedNamesSubscriptionHostedService;
        }

        public async Task<CurrentSanctionedNamesResponse> Handle(GetCurrentSanctionedNamesRequest request, CancellationToken cancellationToken)
        {
            return new CurrentSanctionedNamesResponse
            {
                SanctionedNames = _sanctionedNamesSubscriptionHostedService.GetSanctionedNames()
            };
        }
    }
}
