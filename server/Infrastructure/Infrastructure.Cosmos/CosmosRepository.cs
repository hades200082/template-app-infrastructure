using System.Linq.Expressions;
using Ardalis.GuardClauses;
using Domain.Abstractions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Shared.Core;

namespace Infrastructure.Cosmos;

public sealed class CosmosRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity, new()
{
    private readonly ICosmosProvider _cosmosProvider;
    private readonly ILogger<CosmosRepository<TEntity>> _logger;

    public CosmosRepository(
        ICosmosProvider cosmosProvider,
        ILogger<CosmosRepository<TEntity>> logger)
    {
        _cosmosProvider = cosmosProvider;
        _logger = logger;
    }

    public async Task<TEntity?> FindAsync(string id, string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {id, partitionKeyValue});

        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);
        ItemResponse<TEntity>? result;

        try
        {
            result = await container.ReadItemAsync<TEntity>(id, new PartitionKey(partitionKeyValue),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("find single item", result.RequestCharge, result.Diagnostics);
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {id, partitionKeyValue}, cex);
            return null;
        }

        return result.Resource;
    }

    public Task<IPagedData<TEntity>> QueryAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(null);
        return QueryInternalAsync(null, null, null, 0, cancellationToken);
    }
    public Task<IPagedData<TEntity>> QueryAsync(int maxItems, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {maxItems});
        return QueryInternalAsync(null, null, null, maxItems, cancellationToken);
    }
    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? expression, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression});
        return QueryInternalAsync(expression, null, null, 0, cancellationToken);
    }
    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? expression, int maxItems, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, maxItems});
        return QueryInternalAsync(expression, null, null, maxItems, cancellationToken);
    }
    public Task<IPagedData<TEntity>> QueryAsync(string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {partitionKeyValue});
        return QueryInternalAsync(null, partitionKeyValue, null, 0, cancellationToken);
    }
    public Task<IPagedData<TEntity>> QueryAsync(string partitionKeyValue, int maxItems, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {partitionKeyValue, maxItems});
        return QueryInternalAsync(null, partitionKeyValue, null, maxItems, cancellationToken);
    }
    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? expression, string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, partitionKeyValue});
        return QueryInternalAsync(expression, partitionKeyValue, null, 0, cancellationToken);
    }
    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? expression, string partitionKeyValue, int maxItems, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, partitionKeyValue, maxItems});
        return QueryInternalAsync(expression, partitionKeyValue, null, maxItems, cancellationToken);
    }
    public Task<IPagedData<TEntity>> ContinueQueryAsync(string partitionKeyValue, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {partitionKeyValue, continuationToken});
        return QueryInternalAsync(null, partitionKeyValue, continuationToken, 0, cancellationToken);
    }
    public Task<IPagedData<TEntity>> ContinueQueryAsync(string partitionKeyValue, int maxItems, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {partitionKeyValue, maxItems, continuationToken});
        return QueryInternalAsync(null, partitionKeyValue, continuationToken, maxItems, cancellationToken);
    }
    public Task<IPagedData<TEntity>> ContinueQueryAsync(int maxItems, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {maxItems, continuationToken});
        return QueryInternalAsync(null, null, continuationToken, maxItems, cancellationToken);
    }
    public Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? expression, int maxItems, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, maxItems, continuationToken});
        return QueryInternalAsync(expression, null, continuationToken, maxItems, cancellationToken);
    }
    public Task<IPagedData<TEntity>> ContinueQueryAsync(string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {continuationToken});
        return QueryInternalAsync(null, null, continuationToken, 0, cancellationToken);
    }
    public Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? expression, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, continuationToken});
        return QueryInternalAsync(expression, null, continuationToken, 0, cancellationToken);
    }
    public Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? expression, string partitionKeyValue, int maxItems, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, partitionKeyValue, maxItems, continuationToken});
        return QueryInternalAsync(expression, partitionKeyValue, continuationToken, maxItems, cancellationToken);
    }
    public Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? expression, string partitionKeyValue, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, partitionKeyValue, continuationToken});
        return QueryInternalAsync(expression, partitionKeyValue, continuationToken, 0, cancellationToken);
    }
    private async Task<IPagedData<TEntity>> QueryInternalAsync(Expression<Func<TEntity, bool>>? expression, string? partitionKeyValue, string? continuationToken, int maxItems, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, partitionKeyValue, continuationToken, maxItems});
        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        var options = new QueryRequestOptions();

        if (!string.IsNullOrWhiteSpace(partitionKeyValue))
            options.PartitionKey = new PartitionKey(partitionKeyValue);

        if (maxItems != 0)
            options.MaxItemCount = maxItems;

        try
        {
            var query = container.GetItemLinqQueryable<TEntity>(false, continuationToken, options)
                .Where(x => x.Type == typeof(TEntity).Name);

            if (expression is not null)
                query = query.Where(expression);

            var total = await query.CountAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Total count", total.RequestCharge, total.Diagnostics);

            using var iterator = query.ToFeedIterator();
            if (!iterator.HasMoreResults)
                return new PagedData<TEntity>(Array.Empty<TEntity>(), null!, false, 0);

            var result = await iterator.ReadNextAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Query", result.RequestCharge, result.Diagnostics);
            return new PagedData<TEntity>(result, result.ContinuationToken, iterator.HasMoreResults, total);
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {expression, partitionKeyValue, continuationToken, maxItems}, cex);
            throw new CosmosFriendlyException("Query failed. See inner exception for details.", cex);
        }
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? expression, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression});
        return CountInternalAsync(expression, null, cancellationToken);
    }
    public Task<int> CountAsync(string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {partitionKeyValue});
        return CountInternalAsync(null, partitionKeyValue, cancellationToken);
    }
    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? expression, string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, partitionKeyValue});
        return CountInternalAsync(expression, partitionKeyValue, cancellationToken);
    }
    private async Task<int> CountInternalAsync(Expression<Func<TEntity, bool>>? expression, string? partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, partitionKeyValue});
        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        QueryRequestOptions? options = null;

        if(!string.IsNullOrWhiteSpace(partitionKeyValue))
            options = new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKeyValue),
            };

        try
        {
            Response<int> result;
            var query = container.GetItemLinqQueryable<TEntity>(requestOptions: options);
            if (expression is null)
                result = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            else
                result = await query.Where(expression).CountAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogCosmosDiagnosticsTrace("Count", result.RequestCharge, result.Diagnostics);

            return result.Resource;
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {expression, partitionKeyValue}, cex);
            throw new CosmosFriendlyException("An error occurred while attempting to process a 'count' query in Cosmos. See the inner exception for more details.", cex);
        }
    }

    public async Task<bool> ExistsAsync(string id, string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {id, partitionKeyValue});
        Guard.Against.NullOrWhiteSpace(id);
        Guard.Against.NullOrWhiteSpace(partitionKeyValue);
        return await CountInternalAsync(x => x.Id == id, partitionKeyValue, cancellationToken).ConfigureAwait(false) > 0;
    }
    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {id});
        Guard.Against.NullOrWhiteSpace(id);
        return await CountInternalAsync(x => x.Id == id, null, cancellationToken).ConfigureAwait(false) > 0;
    }
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? expression, string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression, partitionKeyValue});
        Guard.Against.NullOrWhiteSpace(partitionKeyValue);
        return await CountInternalAsync(expression, partitionKeyValue, cancellationToken).ConfigureAwait(false) > 0;
    }
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? expression, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {expression});
        return await CountInternalAsync(expression, null, cancellationToken).ConfigureAwait(false) > 0;
    }

    public async Task<TEntity?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {entity});
        Guard.Against.Null(entity);
        Guard.Against.NullOrWhiteSpace(entity.Id);
        Guard.Against.NullOrWhiteSpace(entity.PartitionKey);

        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var result = await container
                .CreateItemAsync(entity, new PartitionKey(entity.PartitionKey), cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Create item", result.RequestCharge, result.Diagnostics);

            return result.Resource;
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {entity}, cex);
            return null;
        }
    }

    public async Task CreateAsBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var enumeratedEntities = entities.ToList();
        _logger.LogMethodCall(new {enumeratedEntities});
        Guard.Against.NullOrEmpty(enumeratedEntities, nameof(entities));

        if (enumeratedEntities.Select(x => x.PartitionKey).Distinct(StringComparer.Ordinal).Skip(1).Any())
            throw new CosmosFriendlyException(
                "Cannot create as batch unless all items in the batch have the same partition key value");

        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var transaction =
                container.CreateTransactionalBatch(new PartitionKey(enumeratedEntities[0].PartitionKey));

            foreach (var entity in enumeratedEntities)
            {
                transaction.CreateItem(entity);
            }

            using var result = await transaction.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Batch create", result.RequestCharge, result.Diagnostics);
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {enumeratedEntities}, cex);
            throw new CosmosFriendlyException("Batch create failed. See inner exception for details.", cex);
        }
    }

    public async Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {entity});
        Guard.Against.Null(entity);
        Guard.Against.NullOrWhiteSpace(entity.Id);
        Guard.Against.NullOrWhiteSpace(entity.PartitionKey);

        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var result = await container
                .ReplaceItemAsync(entity, entity.Id, new PartitionKey(entity.PartitionKey), cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Update/replace item", result.RequestCharge, result.Diagnostics);

            return result.Resource;
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {entity}, cex);
            return null;
        }
    }

    public async Task<TEntity?> PatchAsync(JsonPatch patch, string id, string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {patch, id, partitionKeyValue});
        Guard.Against.Null(patch);
        Guard.Against.NullOrWhiteSpace(id);
        Guard.Against.NullOrWhiteSpace(partitionKeyValue);

        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var result = await container.PatchItemAsync<TEntity>(id, new PartitionKey(partitionKeyValue),
                patch.ToCosmosPatchOperations(), cancellationToken: cancellationToken).ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Patch", result.RequestCharge, result.Diagnostics);

            return result.Resource;
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {patch, id, partitionKeyValue}, cex);
            throw;
        }
    }

    public Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {entity});
        Guard.Against.Null(entity);
        return DeleteAsync(entity.Id, entity.PartitionKey, cancellationToken);
    }
    public async Task<bool> DeleteAsync(string id, string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new {id, partitionKeyValue});
        Guard.Against.NullOrWhiteSpace(id);
        Guard.Against.NullOrWhiteSpace(partitionKeyValue);

        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var result = await container
                .DeleteItemAsync<TEntity>(id, new PartitionKey(partitionKeyValue), cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Delete item", result.RequestCharge, result.Diagnostics);

            return true;
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {id, partitionKeyValue}, cex);
            return false;
        }
    }

    public Task DeleteAsBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var enumeratedEntities = entities.ToList();
        _logger.LogMethodCall(new { enumerateEntities = enumeratedEntities });
        Guard.Against.NullOrEmpty(enumeratedEntities, nameof(entities));

        var distinctPartitionKeys = enumeratedEntities.Select(x => x.PartitionKey).Distinct(StringComparer.Ordinal).ToList();

        if(!distinctPartitionKeys.Any())
            throw new CosmosFriendlyException(
                "Cannot delete items that have no partition key");

        if (distinctPartitionKeys.Skip(1).Any())
            throw new CosmosFriendlyException(
                "Cannot create as batch unless all items in the batch have the same partition key value");

        return DeleteAsBatchAsync(enumeratedEntities.Select(x => x.Id), distinctPartitionKeys[0], cancellationToken);
    }
    public async Task DeleteAsBatchAsync(IEnumerable<string> ids, string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        var enumeratedIds = ids.ToList();
        _logger.LogMethodCall(new {enumeratedIds, partitionKeyValue});
        Guard.Against.NullOrEmpty(enumeratedIds, nameof(ids));
        Guard.Against.NullOrWhiteSpace(partitionKeyValue);

        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var transaction =
                container.CreateTransactionalBatch(new PartitionKey(partitionKeyValue));

            foreach (var id in enumeratedIds)
            {
                transaction.DeleteItem(id);
            }

            using var result = await transaction.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Batch delete", result.RequestCharge, result.Diagnostics);
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new {enumeratedIds, partitionKeyValue}, cex);
            throw new CosmosFriendlyException("Batch delete failed. See inner exception for details.", cex);
        }
    }
}
