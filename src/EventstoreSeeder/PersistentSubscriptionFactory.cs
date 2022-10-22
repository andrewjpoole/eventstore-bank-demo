using System;
using System.Net.Http;
using EventStore.Client;

namespace eventstore_seeder;

public class PersistentSubscriptionFactory
{
    private EventStorePersistentSubscriptionsClient client;
    private UserCredentials credentials;

    public PersistentSubscriptionFactory()
    {
        credentials = new UserCredentials("admin", "changeit");
        var settings = new EventStoreClientSettings
        {
            CreateHttpMessageHandler = () =>
                new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (_, _, _, _) => true // ignore https
                },
            ConnectivitySettings =
            {
                Address = new Uri("http://localhost:2113"),
                Insecure = true
            },
            DefaultCredentials = credentials,
            ConnectionName = "ajp"
        };
        client = new EventStorePersistentSubscriptionsClient(settings);
    }

    public void Create(string streamName, string groupName, PersistentSubscriptionSettings settings = null)
    {
        settings ??= new PersistentSubscriptionSettings();
        client.CreateAsync(streamName, groupName, settings, null, credentials).GetAwaiter().GetResult();
    }
}