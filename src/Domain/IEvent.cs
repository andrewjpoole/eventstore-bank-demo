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