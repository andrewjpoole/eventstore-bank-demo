using System;
using System.Reflection;

namespace Infrastructure.EventStore.Serialisation;

public interface IDeserialisationTypeMapper
{
    void AddTypesFromAssembly(Assembly assembly);
    Type GetTypeFromName(string typeName);
}