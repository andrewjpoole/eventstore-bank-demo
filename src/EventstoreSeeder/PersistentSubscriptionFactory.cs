using System;
using System.Net.Http;
using Domain;
using EventStore.Client;
using Grpc.Core;

namespace EventstoreSeeder;

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

    public void Create(string streamName, PersistentSubscriptionSettings settings = null)
    {
        try
        {
            var groupName = StreamNames.SubscriptionGroupName(streamName);

            settings ??= new PersistentSubscriptionSettings();
            client.CreateAsync(streamName, groupName, settings, null, credentials).GetAwaiter().GetResult();
            Console.WriteLine($"Created Persistent Subscription for stream {streamName}");
        }
        catch (RpcException e)
        {
            if (e.StatusCode == StatusCode.AlreadyExists)
            {
                Console.WriteLine($"Persistent Subscription for stream {streamName} already exists");
                return;
            }

            Console.WriteLine($"Exception occured while creating Persistent Subscription for stream {streamName} {e}");
            throw;
        }
        
    }
}