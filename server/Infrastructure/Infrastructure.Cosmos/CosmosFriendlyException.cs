namespace Infrastructure.Cosmos;

public sealed class CosmosFriendlyException : Exception
{
    public CosmosFriendlyException(string message) : base(message)
    {
    }

    public CosmosFriendlyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CosmosFriendlyException()
    {
    }
}
