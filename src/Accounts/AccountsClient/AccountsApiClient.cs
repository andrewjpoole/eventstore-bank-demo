﻿using System.Text.Json;
using AccountsDomain.RequestHandlers.Accounts;
using Microsoft.Extensions.Configuration;

namespace AccountsClient;

public class AccountsApiClient : IAccountsApiClient
{
    private readonly IConfiguration _config;
    private HttpClient _client;

    public AccountsApiClient(IConfiguration config)
    {
        _config = config;
        _client = new HttpClient();
        var accountsUrl = config["AccountsApiBaseUrl"];
        _client.BaseAddress = new Uri(accountsUrl);
    }

    public async Task<IEnumerable<AccountSummary>> GetAccountSummaries()
    {
        var response = await _client.GetAsync($"/api/v1/accounts/");

        if (response.IsSuccessStatusCode)
        {
            var responseBodyJson = await response.Content.ReadAsStringAsync();
            var accountSummaries = JsonSerializer.Deserialize<IEnumerable<AccountSummary>>(responseBodyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return accountSummaries;
        }
        
        throw new ApplicationException("Unable to reach the accounts Api");
    }
}