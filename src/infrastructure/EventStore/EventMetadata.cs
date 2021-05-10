using System;

namespace infrastructure.EventStore
{
    public class EventMetadata
    {
        public DateTime Created { get; init; }
        public Guid EventId { get; init; }
        public long EventNumber { get; init; }
    }
}