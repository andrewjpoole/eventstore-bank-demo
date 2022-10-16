using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Events.Accounts;
using Infrastructure.EventStore;
using Microsoft.Extensions.Configuration;

namespace eventstore_seeder;

public class OwnedAccountsSeeder
{
    public List<string> Names = new List<string>
    {
        "Caleb Oliver",
        "Marina Barnes",
        "Emely Perez",
        "Leo Glass",
        "Brenna Thompson",
        "Braden Alvarado",
        "Nikolas Chen",
        "Ella Vasquez",
        "Ian Wolf",
        "Kale Mcknight",
        "Tyson Spears",
        "Edwin Krause",
        "Tilly-Mae Shaffer",
        "Sonny Cano",
        "Lexi-Mae Macias",
        "Juanita Pearce",
        "Florence Herbert",
        "Salahuddin Summers",
        "Arjun Pace",
        "Keely Nielsen",
        "Hadiya East",
        "Willem Weber"
    };

    public async Task Seed()
    {
        Console.WriteLine("Seeding initial managed accounts");

        var random = new Random();
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var publisher = new EventPublisher(new EventStoreClientFactory(config));

        foreach (var name in Names)
        {
            var accountOpened = new AccountOpenedEvent_v1
            {
                Id = Guid.NewGuid(),
                Name = name,
                SortCode = 408011,
                AccountNumber = random.Next(10000000, 99999999),
                Status = AccountStatus.Opened,
                Opened = DateTime.Now.Date.AddDays(random.Next(-60, 0))
            };

            await publisher.Publish(accountOpened, accountOpened.StreamName(), CancellationToken.None);

            Console.WriteLine($"Seeded account with ID:{accountOpened.Id}");
        }

        Console.WriteLine("Completed Seeding initial managed accounts");
    }
}