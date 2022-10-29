namespace payment_scheme_domain.Services;

public class CheckNameResponse
{
    public CheckNameResponse(string name)
    {
        Name = name;
    }

    public string Name { get; init; }
    public bool IsSanctioned { get; init; }
}