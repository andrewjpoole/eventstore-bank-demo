using System;
using EventStore.Client;

namespace infrastructure.EventStore
{
    public class EventMetadata
    {
        public DateTime Created { get; init; }
        public Uuid EventId { get; init; }
        public StreamPosition EventNumber { get; init; }
    }
}