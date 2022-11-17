using AJP.MediatrEndpoints.Exceptions;
using Domain.Interfaces;
using MediatR;
using SanctionsDomain.Events;

namespace SanctionsDomain.RequestHandlers.AddRemoveName;

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
            throw new CustomHttpResponseException("Name must not be empty string", responseStatusCode:400);

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