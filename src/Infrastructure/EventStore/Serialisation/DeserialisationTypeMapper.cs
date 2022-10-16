using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Infrastructure.EventStore.Serialisation;

public class DeserialisationTypeMapper : IDeserialisationTypeMapper
{
    private Dictionary<string, TypeInfo> _domainTypeLookup = new();
    
    public void AddTypesFromAssembly(Assembly assembly)
    {
        _domainTypeLookup = assembly
            .DefinedTypes.Where(t => t.DeclaringType == null)
            .ToDictionary(k => k.Name, v => v);
    }

    public Type GetTypeFromName(string typeName)
    {
        return _domainTypeLookup[typeName] ??
               throw new InvalidOperationException($"Can't find type named {typeName}");
    }
}