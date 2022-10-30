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
        var response = await _postLedgerEntryBehaviour.TryPostLedgerEntry(request);
        return response;
    }
}