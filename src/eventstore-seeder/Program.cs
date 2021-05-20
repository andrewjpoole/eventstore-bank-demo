using System.Threading.Tasks;

namespace eventstore_seeder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var seeder = new OwnedAccountsSeeder();
            //await seeder.SeedInitialManagedAccounts();
            
            var persistentSubscriptionSeeder = new PersistentSubscriptionsSeeder();
            persistentSubscriptionSeeder.Seed();
        }
    }
}
