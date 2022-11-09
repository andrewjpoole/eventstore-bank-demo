using System.Text.Json;
using LedgerDomain.RequestHandlers;
using Microsoft.Extensions.Configuration;

namespace LedgerClient;

public class LedgerApiClient : ILedgerApiClient
{
    private readonly HttpClient _client;

    public LedgerApiClient(IConfiguration config)
    {
        _client = new HttpClient();
        var url = config["LedgerApiBaseUrl"];
        _client.BaseAddress = new Uri(url);
    }

    public async Task<GetBalanceResponse> GetAccountBalance(GetBalanceRequest request)
    {
        // ToDo add polly retry?
        var uri = $"/ledger?SortCode={request.SortCode};AccountNumber={request.AccountNumber}";
        var apiResponse = await _client.GetAsync(uri);

        if (apiResponse.IsSuccessStatusCode)
        {
            var responseBodyJson = await apiResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<GetBalanceResponse>(responseBodyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                           ?? throw new InvalidOperationException("Unable to deserialise ledger Api response into GetBalanceResponse");

            return response;
        }

        throw new ApplicationException("Unable to reach the ledger Api");
    }

    public async Task<PostLedgerEntryResponse> PostLedgerEntry(PostLedgerEntryRequest request)
    {
        // ToDo add polly retry?
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var bodyJson = JsonSerializer.Serialize(request, options);
        var apiResponse = await _client.PostAsync("/ledger", new StringContent(bodyJson));

        if (apiResponse.IsSuccessStatusCode)
        {
            var responseBodyJson = await apiResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<PostLedgerEntryResponse>(responseBodyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                           ?? throw new InvalidOperationException("Unable to deserialise ledger Api response into PostLedgerEntryResponse");

            return response;
        }

        throw new ApplicationException("Unable to reach the ledger Api");
    }
}