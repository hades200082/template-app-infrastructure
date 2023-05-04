namespace Infrastructure.Storage;

public sealed class StorageOptions
{
    public StorageOptions(string accountKey, string accountName)
    {
        AccountKey = accountKey;
        AccountName = accountName;
    }

    public string AccountKey { get; init; }
    public string AccountName { get; init; }
}