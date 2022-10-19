using System;
using System.Collections.Generic;
using OneOf;
using OneOf.Types;

namespace Domain;

public interface IEvent
{
    string StreamName();
    int Version();
    OneOf<True, List<string>> IsValid();
}

public interface IEventWrapper
{
    string EventJson { get; }
    string EventTypeName { get; }
    DateTime Created { get; }
    Guid EventId { get; }
    long EventNumber { get; }
    string Version { get; }
    dynamic Metadata { get; }
}