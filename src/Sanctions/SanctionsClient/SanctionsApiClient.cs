using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OneOf;
using OneOf.Types;
using SanctionsDomain.RequestHandlers.CheckName;

namespace SanctionsClient;

public class SanctionsApiClient : ISanctionsApiClient
{
    private readonly HttpClient _client;

    public SanctionsApiClient(IConfiguration config)
    {
        _client = new HttpClient();
        var sanctionsUrl = config["SanctionsApiBaseUrl"];
        _client.BaseAddress = new Uri(sanctionsUrl);
    }

    public async Task<OneOf<False, string>> CheckIfNameIsSanctioned(string? name)
    {
        // ToDo add polly retry?
        var body = new
        {
            name
        };

        var bodyJson = JsonSerializer.Serialize(body);
        var response = await _client.PostAsync("/sanctioned-names/check-name", new StringContent(bodyJson));

        if (response.IsSuccessStatusCode)
        {
            var responseBodyJson = await response.Content.ReadAsStringAsync();
            var checkNameResponse =
                JsonSerializer.Deserialize<CheckNameResponse>(responseBodyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                ?? throw new InvalidOperationException("Unable to deserialise sanctions check names response");

            if (checkNameResponse.IsSanctioned)
                return $"{checkNameResponse.Name} appears in current sanctioned names list.";
            
            return new False();
        }
        
        throw new ApplicationException("Unable to reach the sanctions Api");
    }
}