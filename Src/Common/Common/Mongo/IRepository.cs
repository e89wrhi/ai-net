using AI.Common.Core;
using System.Linq.Expressions;

namespace AI.Common.Mongo;

/// <summary>
/// A read-only repository interface.
/// Provides methods for querying data without modification capabilities.
/// </summary>
public interface IReadRepository<TEntity, in TId>
    where TEntity : class, IAggregate<TId>
{
    /// <summary>
    /// Finds an entity by its unique identifier.
    /// </summary>
    Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a single entity that matches the given predicate.
    /// </summary>
    Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds all entities that match the given predicate.
    /// </summary>
    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all entities from the store.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a raw query against the data store.
    /// </summary>
    public Task<IReadOnlyList<TEntity>> RawQuery(
        string query,
        CancellationToken cancellationToken = default,
        params object[] queryParams);
}

/// <summary>
/// A write-only repository interface.
/// Provides methods for creating, updating, and deleting data.
/// </summary>
public interface IWriteRepository<TEntity, in TId>
    where TEntity : class, IAggregate<TId>
{
    /// <summary>
    /// Adds a new entity to the data store.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the data store.
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a range of entities from the data store.
    /// </summary>
    Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all entities that match the given predicate.
    /// </summary>
    Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific entity from the data store.
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default);
}

/// <summary>
/// A full repository interface that combines read and write capabilities.
/// </summary>
public interface IRepository<TEntity, in TId> :
    IReadRepository<TEntity, TId>,
    IWriteRepository<TEntity, TId>,
    IDisposable
    where TEntity : class, IAggregate<TId>
{
}

/// <summary>
/// A default repository interface for entities using a long ID.
/// </summary>
public interface IRepository<TEntity> : IRepository<TEntity, long>
    where TEntity : class, IAggregate<long>
{
}