using Microsoft.Azure.Cosmos;

namespace Infrastructure.Cosmos;

public interface IChangeFeedProvider
{
    Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync<T>(string processorName, CancellationToken cancellationToken = default);
}
