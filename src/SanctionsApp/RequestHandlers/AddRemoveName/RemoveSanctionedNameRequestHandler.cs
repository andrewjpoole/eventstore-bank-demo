using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Events.Sanctions;
using Infrastructure.EventStore;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace sanctions_api.RequestHandlers.AddRemoveName;

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

        var removeSanctionedNameEvent = new SanctionedNameRemoved_v1()
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