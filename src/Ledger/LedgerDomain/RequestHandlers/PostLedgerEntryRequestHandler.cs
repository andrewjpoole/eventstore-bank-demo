using LedgerDomain.Behaviours;
using MediatR;

namespace LedgerDomain.RequestHandlers;

public class PostLedgerEntryRequestHandler : IRequestHandler<PostLedgerEntryRequest, PostLedgerEntryResponse>
{
    private readonly IPostLedgerEntryBehaviour _postLedgerEntryBehaviour;

    public PostLedgerEntryRequestHandler(IPostLedgerEntryBehaviour postLedgerEntryBehaviour)
    {
        _postLedgerEntryBehaviour = postLedgerEntryBehaviour;
    }

    public async Task<PostLedgerEntryResponse> Handle(PostLedgerEntryRequest request, CancellationToken cancellationToken)
    {
        if (request.CorrelationId == Guid.Empty
            || request.PaymentId == Guid.Empty
            || request.DestinationSortCode == 0
            || request.DestinationAccountNumber == 0)
            throw new InvalidOperationException("Mandatory fields missing on request");

        var response = await _postLedgerEntryBehaviour.TryPostLedgerEntry(request);
        return response;
    }
}