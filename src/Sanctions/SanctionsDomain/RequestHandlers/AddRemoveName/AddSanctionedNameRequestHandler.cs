using AJP.MediatrEndpoints.Exceptions;
using Domain.Interfaces;
using MediatR;
using SanctionsDomain.Events;

namespace SanctionsDomain.RequestHandlers.AddRemoveName;

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
            throw new CustomHttpResponseException("Name must not be empty string", responseStatusCode:400); // ToDo should have to know about response codes etc here

        var addSanctionedNameEvent = new SanctionedNameAdded_v1
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