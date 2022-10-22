﻿using System;

namespace Domain;

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