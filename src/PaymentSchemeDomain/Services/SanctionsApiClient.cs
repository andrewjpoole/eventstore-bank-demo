using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OneOf;
using OneOf.Types;

namespace payment_scheme_domain.Services;

public class SanctionsApiClient : ISanctionsApiClient
{
    private readonly IConfiguration _config;
    private HttpClient _client;

    public SanctionsApiClient(IConfiguration config)
    {
        _config = config;
        _client = new HttpClient();
        var sanctionsUrl = config["SanctionsApiBaseUrl"];
        _client.BaseAddress = new Uri(sanctionsUrl);
    }

    public async Task<bool> CheckIfNameIsSanctioned(string name)
    {
        // ToDo add polly retry?
        var body = new
        {
            name
        };

        var bodyJson = JsonSerializer.Serialize(body);
        var response = await _client.PostAsync($"/sanctions/check-name", new StringContent(bodyJson));

        if (response.IsSuccessStatusCode)
        {
            var responseBodyJson = await response.Content.ReadAsStringAsync();
            var checkNameResponse = JsonSerializer.Deserialize<CheckNameResponse>(responseBodyJson, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

            return checkNameResponse.IsSanctioned;
        }

        // retries?
        // throw on error?
        throw new ApplicationException("Unable to reach the sanctions Api");
    }

    public async Task<OneOf<False, string>> CheckIfNameIsSanctioned2(string name)
    {
        // ToDo add polly retry?
        var body = new
        {
            name
        };

        var bodyJson = JsonSerializer.Serialize(body);
        var response = await _client.PostAsync($"/sanctions/check-name", new StringContent(bodyJson));

        if (response.IsSuccessStatusCode)
        {
            var responseBodyJson = await response.Content.ReadAsStringAsync();
            var checkNameResponse = JsonSerializer.Deserialize<CheckNameResponse>(responseBodyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (checkNameResponse.IsSanctioned)
                return $"{name} appears in current sanctioned names list.";
            
            return new False();
        }
        
        throw new ApplicationException("Unable to reach the sanctions Api");
    }
}