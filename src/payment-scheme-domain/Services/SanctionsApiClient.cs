using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace payment_scheme_domain.Services
{
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
            var response = await _client.GetAsync($"/sanctions/check-name?Name={name}");

            if (response.IsSuccessStatusCode)
            {
                var responseBodyJson = await response.Content.ReadAsStringAsync();
                var checkNameResponse = JsonSerializer.Deserialize<CheckNameResponse>(responseBodyJson);

                return checkNameResponse.IsSanctioned;
            }

            // retries?
            // throw on error?
            throw new ApplicationException("Unable to reach the sanctions Api");
        }
    }
}