using System.Threading.Tasks;

namespace eventstore_seeder;

class Program
{
    static async Task Main(string[] args)
    {
        var accountsSeeder = new OwnedAccountsSeeder();
        await accountsSeeder.Seed();

        var sanctionedNamesSeeder = new SanctionedNameSeeder();
        await sanctionedNamesSeeder.Seed();

        var persistentSubscriptionSeeder = new PersistentSubscriptionsSeeder();
        persistentSubscriptionSeeder.Seed();

        // TODO add user projections - $projections-SanctionedPayments
    }
}