using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace events.Sanctions
{
    public class SanctionedNameAdded_V1 : IEvent
    {
        public string SanctionedName { get; init; }
        public DateTime Added { get; init; }

        public string StreamName() => SubscriptionNames.Sanctions.GlobalSanctionedNames;
        public int  Version() => 1;
    }

    public class SanctionedNameRemoved_V1 : IEvent
    {
        public string SanctionedName { get; init; }
        public DateTime Removed { get; init; }

        public string StreamName() => SubscriptionNames.Sanctions.GlobalSanctionedNames;
        public int Version() => 1;
    }

    // add snapshot options?
    // 1) event with a list of strings in the eventData
    // 2) if likely to get near the 4Mb event size limit, link snapshots in a sequence?
    // 3) OR event which points to a blob which contains the snapshoted json data to load
}
