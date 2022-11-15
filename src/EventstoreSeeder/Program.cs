using System.Threading.Tasks;

namespace EventstoreSeeder;

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
    }
}