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
        var newTypes = assembly
            .DefinedTypes.Where(t => t.DeclaringType == null 
                                     && !t.Namespace.StartsWith("Microsoft")
                                     && !t.Namespace.StartsWith("System"))
            .ToDictionary(k => k.Name, v => v);

        foreach (var typeInfo in newTypes)
        {
            if (_domainTypeLookup.ContainsKey(typeInfo.Key))
                throw new NotSupportedException($"Types registered in the DeserialisationTypeMapper must have unique names, a type named {typeInfo.Key} has already been added to the dictionary.");

            _domainTypeLookup.Add(typeInfo.Key, typeInfo.Value);
        }
    }

    public Type GetTypeFromName(string typeName)
    {
        return _domainTypeLookup[typeName] ??
               throw new InvalidOperationException($"Can't find type named {typeName}");
    }
}