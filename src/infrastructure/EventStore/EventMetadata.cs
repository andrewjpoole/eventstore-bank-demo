using System;
using EventStore.Client;

namespace Infrastructure.EventStore;

public class EventMetadata
{
    public DateTime Created { get; init; }
    public Guid EventId { get; init; }
    public StreamPosition EventNumber { get; init; }
    // ToDo add correlationId and CausationId
}