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

    public async Task<TEntity?> FindAsync(string id, string partitionKeyValue,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { id, partitionKeyValue });

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
            _logger.LogCosmosException(new { id, partitionKeyValue }, cex);
            throw new CosmosFriendlyException("Query failed. See inner exception for details.", cex);
        }

        return result.Resource;
    }

    public Task<IPagedData<TEntity>> QueryAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(null);
        return QueryInternalAsync(null, null, null, 0, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, dynamic>> orderBySelector,
        string orderByDirection = "ASC", CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { orderBySelector, orderByDirection });
        return QueryInternalAsync(null, null, null, 0, orderBySelector, orderByDirection,
            cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(int maxItems, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { maxItems });
        return QueryInternalAsync(null, null, null, maxItems, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(int maxItems, Expression<Func<TEntity, dynamic>> orderBySelector,
        string orderByDirection = "ASC", CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { maxItems, orderBySelector, orderByDirection });
        return QueryInternalAsync(null, null, null, maxItems, orderBySelector, orderByDirection,
            cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression });
        return QueryInternalAsync(whereExpression, null, null, 0, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression,
        Expression<Func<TEntity, dynamic>> orderBySelector,
        string orderByDirection = "ASC", CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, orderBySelector, orderByDirection });
        return QueryInternalAsync(whereExpression, null, null, 0, orderBySelector, orderByDirection, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression, int maxItems,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, maxItems });
        return QueryInternalAsync(whereExpression, null, null, maxItems, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression, int maxItems,
        Expression<Func<TEntity, dynamic>> orderBySelector,
        string orderByDirection = "ASC", CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, maxItems, orderBySelector, orderByDirection });
        return QueryInternalAsync(whereExpression, null, null, maxItems, orderBySelector, orderByDirection, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { partitionKeyValue });
        return QueryInternalAsync(null, partitionKeyValue, null, 0, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(string partitionKeyValue, Expression<Func<TEntity, dynamic>> orderBySelector,
        string orderByDirection = "ASC", CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { partitionKeyValue, orderBySelector, orderByDirection });
        return QueryInternalAsync(null, partitionKeyValue, null, 0, orderBySelector, orderByDirection, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(string partitionKeyValue, int maxItems,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { partitionKeyValue, maxItems });
        return QueryInternalAsync(null, partitionKeyValue, null, maxItems, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(string partitionKeyValue, int maxItems,
        Expression<Func<TEntity, dynamic>> orderBySelector,
        string orderByDirection = "ASC", CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { partitionKeyValue, maxItems, orderBySelector, orderByDirection });
        return QueryInternalAsync(null, partitionKeyValue, null, maxItems, orderBySelector, orderByDirection, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression,
        string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, partitionKeyValue });
        return QueryInternalAsync(whereExpression, partitionKeyValue, null, 0, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression,
        string partitionKeyValue, int maxItems, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, partitionKeyValue, maxItems });
        return QueryInternalAsync(whereExpression, partitionKeyValue, null, maxItems,
            cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> ContinueQueryAsync(string partitionKeyValue, string continuationToken,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { partitionKeyValue, continuationToken });
        return QueryInternalAsync(null, partitionKeyValue, continuationToken, 0, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> ContinueQueryAsync(string partitionKeyValue, int maxItems,
        string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { partitionKeyValue, maxItems, continuationToken });
        return QueryInternalAsync(null, partitionKeyValue, continuationToken, maxItems,
            cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> ContinueQueryAsync(int maxItems, string continuationToken,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { maxItems, continuationToken });
        return QueryInternalAsync(null, null, continuationToken, maxItems, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? whereExpression, int maxItems,
        string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, maxItems, continuationToken });
        return QueryInternalAsync(whereExpression, null, continuationToken, maxItems,
            cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> ContinueQueryAsync(string continuationToken,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { continuationToken });
        return QueryInternalAsync(null, null, continuationToken, 0, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? whereExpression,
        string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, continuationToken });
        return QueryInternalAsync(whereExpression, null, continuationToken, 0, cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? whereExpression,
        string partitionKeyValue, int maxItems, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, partitionKeyValue, maxItems, continuationToken });
        return QueryInternalAsync(whereExpression, partitionKeyValue, continuationToken, maxItems,
            cancellationToken: cancellationToken);
    }

    public Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? whereExpression,
        string partitionKeyValue, string continuationToken, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, partitionKeyValue, continuationToken });
        return QueryInternalAsync(whereExpression, partitionKeyValue, continuationToken, 0,
            cancellationToken: cancellationToken);
    }

    private async Task<IPagedData<TEntity>> QueryInternalAsync(
        Expression<Func<TEntity, bool>>? whereExpression,
        string? partitionKeyValue,
        string? continuationToken,
        int maxItems,
        Expression<Func<TEntity, dynamic>>? orderBySelector = null,
        string orderByDirection = "ASC",
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, partitionKeyValue, continuationToken, maxItems });
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

            if (whereExpression is not null)
                query = query.Where(whereExpression);

            if (orderBySelector is not null && !string.IsNullOrWhiteSpace(orderByDirection))
                query = orderByDirection.ToUpperInvariant() switch
                {
                    "ASC" => query.OrderBy(orderBySelector),
                    "DESC" => query.OrderByDescending(orderBySelector),
                    _ => throw new ArgumentOutOfRangeException(nameof(orderByDirection))
                };

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
            _logger.LogCosmosException(new { whereExpression, partitionKeyValue, continuationToken, maxItems }, cex);
            throw new CosmosFriendlyException("Query failed. See inner exception for details.", cex);
        }
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? whereExpression,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression });
        return CountInternalAsync(whereExpression, null, cancellationToken);
    }

    public Task<int> CountAsync(string partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { partitionKeyValue });
        return CountInternalAsync(null, partitionKeyValue, cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>>? whereExpression, string partitionKeyValue,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, partitionKeyValue });
        return CountInternalAsync(whereExpression, partitionKeyValue, cancellationToken);
    }

    private async Task<int> CountInternalAsync(Expression<Func<TEntity, bool>>? whereExpression,
        string? partitionKeyValue, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, partitionKeyValue });
        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        QueryRequestOptions? options = null;

        if (!string.IsNullOrWhiteSpace(partitionKeyValue))
            options = new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKeyValue), };

        try
        {
            Response<int> result;
            var query = container.GetItemLinqQueryable<TEntity>(requestOptions: options);
            if (whereExpression is null)
                result = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            else
                result = await query.Where(whereExpression).CountAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogCosmosDiagnosticsTrace("Count", result.RequestCharge, result.Diagnostics);

            return result.Resource;
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new { whereExpression, partitionKeyValue }, cex);
            throw new CosmosFriendlyException(
                "An error occurred while attempting to process a 'count' query in Cosmos. See the inner exception for more details.",
                cex);
        }
    }

    public async Task<bool> ExistsAsync(string id, string partitionKeyValue,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { id, partitionKeyValue });
        Guard.Against.NullOrWhiteSpace(id);
        Guard.Against.NullOrWhiteSpace(partitionKeyValue);
        return await CountInternalAsync(x => x.Id == id, partitionKeyValue, cancellationToken).ConfigureAwait(false) >
               0;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { id });
        Guard.Against.NullOrWhiteSpace(id);
        return await CountInternalAsync(x => x.Id == id, null, cancellationToken).ConfigureAwait(false) > 0;
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? whereExpression, string partitionKeyValue,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression, partitionKeyValue });
        Guard.Against.NullOrWhiteSpace(partitionKeyValue);
        return await CountInternalAsync(whereExpression, partitionKeyValue, cancellationToken).ConfigureAwait(false) >
               0;
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? whereExpression,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { whereExpression });
        return await CountInternalAsync(whereExpression, null, cancellationToken).ConfigureAwait(false) > 0;
    }

    public async Task<TEntity?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { entity });
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
            _logger.LogCosmosException(new { entity }, cex);
            return null;
        }
    }

    public async Task CreateAsBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var enumeratedEntities = entities.ToList();
        _logger.LogMethodCall(new { enumeratedEntities });
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
            _logger.LogCosmosException(new { enumeratedEntities }, cex);
            throw new CosmosFriendlyException("Batch create failed. See inner exception for details.", cex);
        }
    }

    public async Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { entity });
        Guard.Against.Null(entity);
        Guard.Against.NullOrWhiteSpace(entity.Id);
        Guard.Against.NullOrWhiteSpace(entity.PartitionKey);

        var container = await _cosmosProvider.GetContainerAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var result = await container
                .ReplaceItemAsync(entity, entity.Id, new PartitionKey(entity.PartitionKey),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            _logger.LogCosmosDiagnosticsTrace("Update/replace item", result.RequestCharge, result.Diagnostics);

            return result.Resource;
        }
        catch (CosmosException cex)
        {
            _logger.LogCosmosException(new { entity }, cex);
            return null;
        }
    }

    public async Task<TEntity?> PatchAsync(JsonPatch patch, string id, string partitionKeyValue,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { patch, id, partitionKeyValue });
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
            _logger.LogCosmosException(new { patch, id, partitionKeyValue }, cex);
            throw;
        }
    }

    public Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { entity });
        Guard.Against.Null(entity);
        return DeleteAsync(entity.Id, entity.PartitionKey, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, string partitionKeyValue,
        CancellationToken cancellationToken = default)
    {
        _logger.LogMethodCall(new { id, partitionKeyValue });
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
            _logger.LogCosmosException(new { id, partitionKeyValue }, cex);
            return false;
        }
    }

    public Task DeleteAsBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var enumeratedEntities = entities.ToList();
        _logger.LogMethodCall(new { enumerateEntities = enumeratedEntities });
        Guard.Against.NullOrEmpty(enumeratedEntities, nameof(entities));

        var distinctPartitionKeys =
            enumeratedEntities.Select(x => x.PartitionKey).Distinct(StringComparer.Ordinal).ToList();

        if (!distinctPartitionKeys.Any())
            throw new CosmosFriendlyException(
                "Cannot delete items that have no partition key");

        if (distinctPartitionKeys.Skip(1).Any())
            throw new CosmosFriendlyException(
                "Cannot create as batch unless all items in the batch have the same partition key value");

        return DeleteAsBatchAsync(enumeratedEntities.Select(x => x.Id), distinctPartitionKeys[0], cancellationToken);
    }

    public async Task DeleteAsBatchAsync(IEnumerable<string> ids, string partitionKeyValue,
        CancellationToken cancellationToken = default)
    {
        var enumeratedIds = ids.ToList();
        _logger.LogMethodCall(new { enumeratedIds, partitionKeyValue });
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
            _logger.LogCosmosException(new { enumeratedIds, partitionKeyValue }, cex);
            throw new CosmosFriendlyException("Batch delete failed. See inner exception for details.", cex);
        }
    }
}
