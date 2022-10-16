using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Events.Sanctions;
using Infrastructure.EventStore;
using Microsoft.Extensions.Configuration;

namespace eventstore_seeder;

public class SanctionedNameSeeder
{
    public List<string> SanctionedNames = new List<string>
    {
        "Osama Bin Laden",
        "Donald Trump",
        "Borris Johnson"
    };

    public async Task Seed()
    {
        Console.WriteLine("Seeding initial sanctioned names list");

        var random = new Random();
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var publisher = new EventPublisher(new EventStoreClientFactory(config));

        foreach (var name in SanctionedNames)
        {
            var sanctionedNameAdded = new SanctionedNameAdded_v1
            {
                Added = DateTime.Now,
                SanctionedName = name
            };

            await publisher.Publish(sanctionedNameAdded, sanctionedNameAdded.StreamName(), CancellationToken.None);

            Console.WriteLine($"Added Name:{sanctionedNameAdded.SanctionedName} to sanctioned names list");
        }

        Console.WriteLine("Completed Seeding initial sanctioned names list");
    }
}