using Domain;
using Domain.Interfaces;
using OneOf;
using OneOf.Types;

namespace SanctionsDomain.Events;

public class SanctionedNameRemoved_v1 : IEvent
{
    public string SanctionedName { get; init; }
    public DateTime Removed { get; init; }

    public string StreamName() => SanctionsDomainStreamNames.GlobalSanctionedNames;
    public int Version() => 1;
    public OneOf<True, List<string>> IsValid() => new True();
}

// add snapshot options?
// 1) event with a list of strings in the eventData
// 2) if likely to get near the 4Mb event size limit, link snapshots in a sequence?
// 3) OR event which points to a blob which contains the snapshotted json data to load