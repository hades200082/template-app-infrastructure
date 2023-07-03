namespace Infrastructure.Identity.Auth0;

internal sealed record Auth0Options(string Domain, string NameClaimType,  string[] ValidAudiences)
{
    public Auth0Options()
        : this(string.Empty, string.Empty, Array.Empty<string>())
    {
    }

    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(Domain)
               && !string.IsNullOrWhiteSpace(NameClaimType)
               && !ValidAudiences.Any(string.IsNullOrWhiteSpace);
    }
}
