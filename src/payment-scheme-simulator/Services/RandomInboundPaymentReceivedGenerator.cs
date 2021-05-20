using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using accounts_api.RequestHandlers.Accounts;
using events.Payments;

namespace payment_scheme_simulator.Services
{
    public class RandomInboundPaymentReceivedGenerator : IRandomInboundPaymentReceivedGenerator
    {
        public async Task<InboundPaymentReceived_v1> Generate(PaymentScheme scheme, PaymentType type)
        {
            var random = new Random();

            var ownedAccount = GetRandomOwnedAccountFromList(random);
            var externalAccount = await GetRandomExternalAccountDetails(random);

            // TODO switch accounts around according to type?

            var @event = new InboundPaymentReceived_v1
            {
                CorrelationId = Guid.NewGuid(),
                Amount = decimal.Parse($"{random.Next(1, 1_000_000)}.{random.Next(0, 99)}"),
                PaymentReference = $"Simulated inbound payment {Guid.NewGuid().ToString().Substring(0, 6)}",
                ProcessingDate = DateTime.Now.Date,
                Scheme = scheme,
                Type = type,
                OriginatingSortCode = externalAccount.SortCode,
                OriginatingAccountNumber = externalAccount.AccountNumber,
                OriginatingAccountName = externalAccount.Name,
                DestinationSortCode = ownedAccount.SortCode,
                DestinationAccountNumber = ownedAccount.AccountNumber,
                DestinationAccountName = ownedAccount.Name
            };

            return @event;
        }

        private (string Name, int SortCode, int AccountNumber) GetRandomOwnedAccountFromList(Random random)
        {
            var existingAccountsJson = File.ReadAllText(@"c:\temp\accounts.json");
            var existingAccounts = JsonSerializer.Deserialize<List<AccountSummary>>(existingAccountsJson, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});

            var randomIndex = random.Next(0, existingAccounts.Count() - 1);
            var existingAccount = existingAccounts[randomIndex];
            return (existingAccount.AccountName, existingAccount.SortCode, existingAccount.AccountNumber);
        }

        private async Task<(string Name, int SortCode, int AccountNumber)> GetRandomExternalAccountDetails(Random random)
        {
            string name;
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://randomuser.me");
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/?nat=gb"));
            if (response.IsSuccessStatusCode)
            {
                var randomUserJson = await response.Content.ReadAsStringAsync();
                var randomUser = JsonSerializer.Deserialize<RandomUserResult>(randomUserJson);
                name = $"{randomUser.results.First().name.first} {randomUser.results.First().name.last}";

                return (name, random.Next(100000, 999999), random.Next(10000000, 99999999));
            }
            else
            {
                throw new ApplicationException("Could not retrieve random user");
            }
        }
    }
 
    public class Name
    {
        public string title { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }

    public class Street
    {
        public int number { get; set; }
        public string name { get; set; }
    }
    
    public class Location
    {
        public Street street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postcode { get; set; }
    }

    public class Result
    {
        public Name name { get; set; }
        public Location location { get; set; }
        public string email { get; set; }
    }

    public class RandomUserResult
    {
        public List<Result> results { get; set; }
    }


}