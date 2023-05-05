namespace Infrastructure.Cosmos;

public sealed class CosmosOptions
{
    public string? AccountEndpoint { get; set; }
    public string? AccountKey { get; set; }
    public string? DatabaseId { get; set; }
    public string? ContainerName { get; set; }

    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(AccountEndpoint)
               && !string.IsNullOrWhiteSpace(AccountKey)
               && !string.IsNullOrWhiteSpace(DatabaseId)
               && !string.IsNullOrWhiteSpace(ContainerName);
    }
}
