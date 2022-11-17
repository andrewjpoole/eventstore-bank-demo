namespace SanctionsDomain.ServiceInterfaces;

public interface ISanctionedNamesCatchupHostedService
{
    List<string> GetSanctionedNames();
}