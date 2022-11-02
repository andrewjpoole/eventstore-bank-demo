using LedgerDomain.RequestHandlers;

namespace LedgerClient;

public interface ILedgerApiClient
{
    Task<GetBalanceResponse> GetAccountBalance(GetBalanceRequest request);
    Task<PostLedgerEntryResponse> PostLedgerEntry(PostLedgerEntryRequest request);
}