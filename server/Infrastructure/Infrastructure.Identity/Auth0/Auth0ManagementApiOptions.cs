namespace Infrastructure.Identity.Auth0;

internal sealed record Auth0ManagementApiOptions(string ClientId, string ClientSecret, string Domain)
{
    public Auth0ManagementApiOptions() : this(string.Empty,string.Empty, string.Empty)
    {
    }

    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(ClientId)
               && !string.IsNullOrWhiteSpace(ClientSecret)
               && !string.IsNullOrWhiteSpace(Domain);
    }
}
