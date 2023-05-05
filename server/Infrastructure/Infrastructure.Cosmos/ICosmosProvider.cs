using Microsoft.Azure.Cosmos;

namespace Infrastructure.Cosmos;

public interface ICosmosProvider
{
    Task<Container> GetContainerAsync(CancellationToken cancellationToken = default);
    Task<Container> GetLeaseContainerAsync(CancellationToken cancellationToken);
}
