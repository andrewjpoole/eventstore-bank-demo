using System;

namespace Domain;

public class SharedStreamNames
{
    public static string SubscriptionGroupName(string streamName) => $"{streamName}-SubscriptionGroup"; // Todo place a number 10,20,30 in this name so the subscriptions are ordered in the eventstore UI?
}