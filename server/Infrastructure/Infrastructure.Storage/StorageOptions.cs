namespace Infrastructure.Storage;

public sealed class StorageOptions
{
    public string AccountKey { get; init; }
    public string AccountName { get; init; }
    public string ContainerName { get; init; }
}
