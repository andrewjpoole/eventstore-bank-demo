namespace SanctionsDomain.RequestHandlers.CheckName;

public class CheckNameResponse
{
    public string Name { get; init; }
    public bool IsSanctioned { get; init; }
}