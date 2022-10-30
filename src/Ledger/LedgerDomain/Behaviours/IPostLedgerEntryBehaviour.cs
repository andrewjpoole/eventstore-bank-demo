using LedgerDomain.RequestHandlers;

namespace LedgerDomain.Behaviours;

public interface IPostLedgerEntryBehaviour
{
    Task<PostLedgerEntryResponse> TryPostLedgerEntry(PostLedgerEntryRequest request);
}