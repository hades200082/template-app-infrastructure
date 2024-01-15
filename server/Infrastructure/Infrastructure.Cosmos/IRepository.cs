using System.Linq.Expressions;
using Domain.Abstractions;
using Shared.Core;

namespace Infrastructure.Cosmos;

public interface IRepository<TEntity>
    where TEntity : class, IEntity, new()
{
    // Find (direct in partition - single result or nothing)
    Task<TEntity?> FindAsync(string id, string partitionKeyValue, CancellationToken cancellationToken = default);

    // Get (cross partition)
    Task<IPagedData<TEntity>> QueryAsync(CancellationToken cancellationToken = default); // Effectively "Get Everything" from the container
    Task<IPagedData<TEntity>> QueryAsync(int maxItems, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression, int maxItems, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> QueryAsync(string partitionKeyValue, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> QueryAsync(string partitionKeyValue, int maxItems, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression, string partitionKeyValue, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? whereExpression, string partitionKeyValue, int maxItems, CancellationToken cancellationToken = default);

    Task<IPagedData<TEntity>> ContinueQueryAsync(string continuationToken, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> ContinueQueryAsync(int maxItems, string continuationToken, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? whereExpression, string continuationToken, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? whereExpression, int maxItems, string continuationToken, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> ContinueQueryAsync(string partitionKeyValue, string continuationToken, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> ContinueQueryAsync(string partitionKeyValue, int maxItems, string continuationToken, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? whereExpression, string partitionKeyValue, string continuationToken, CancellationToken cancellationToken = default);
    Task<IPagedData<TEntity>> ContinueQueryAsync(Expression<Func<TEntity, bool>>? whereExpression, string partitionKeyValue, int maxItems, string continuationToken, CancellationToken cancellationToken = default);


    // Count
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? whereExpression, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string partitionKeyValue, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? whereExpression, string partitionKeyValue, CancellationToken cancellationToken = default);

    // Exists
    Task<bool> ExistsAsync(string id, string partitionKeyValue, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? whereExpression, string partitionKeyValue, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? whereExpression, CancellationToken cancellationToken = default);

    // Create
    Task<TEntity?> CreateAsync(TEntity entity,CancellationToken cancellationToken = default);

    // Update
    Task<TEntity?> UpdateAsync(TEntity entity,CancellationToken cancellationToken = default);

    // Patch
    Task<TEntity?> PatchAsync(JsonPatch patch, string id, string partitionKeyValue, CancellationToken cancellationToken = default);

    // Delete
    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, string partitionKeyValue, CancellationToken cancellationToken = default);

    // Batch operations
    Task CreateAsBatchAsync(IEnumerable<TEntity> entities,CancellationToken cancellationToken = default);
    Task UpdateAsBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task DeleteAsBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}
