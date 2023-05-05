using Domain.Abstractions;
using Microsoft.Azure.Cosmos;

namespace Infrastructure.Cosmos;

public interface IChangeFeedHandler
{
    Task HandleChangeFeedAsync(ChangeFeedProcessorContext context, IReadOnlyCollection<Entity> changes, CancellationToken cancellationtoken);
}
