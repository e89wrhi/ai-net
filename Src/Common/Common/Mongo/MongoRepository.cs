using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using AI.Common.Core;
using System.Linq.Expressions;

namespace AI.Common.Mongo;

/// <summary>
/// A concrete implementation of the generic repository interface for MongoDB.
/// It wraps the standard MongoDB driver methods to provide a consistent repository API.
/// </summary>
public class MongoRepository<TEntity, TId> : IMongoRepository<TEntity, TId>
    where TEntity : class, IAggregate<TId>
{
    private readonly IMongoDbContext _context;
    protected readonly IMongoCollection<TEntity> DbSet;

    public MongoRepository(IMongoDbContext context)
    {
        _context = context;
        DbSet = _context.GetCollection<TEntity>();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    /// <summary>
    /// Finds an entity by its unique identifier.
    /// </summary>
    public Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return FindOneAsync(e => e.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Finds a single entity that matches the given predicate.
    /// </summary>
    public Task<TEntity?> FindOneAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Find(predicate).SingleOrDefaultAsync(cancellationToken: cancellationToken)!;
    }

    /// <summary>
    /// Finds all entities that match the given predicate.
    /// </summary>
    public async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.Find(predicate).ToListAsync(cancellationToken: cancellationToken)!;
    }

    /// <summary>
    /// Retrieves all entities from the collection.
    /// </summary>
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsQueryable().ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TEntity>> RawQuery(
        string query,
        CancellationToken cancellationToken = default,
        params object[] queryParams)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Queues the addition of a new entity. 
    /// Note: MongoDB implementation here uses the Command pattern, adding the operation to the context's queue.
    /// </summary>
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.AddCommand(() => DbSet.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken));

        return entity;
    }

    /// <summary>
    /// Queues the update of an existing entity.
    /// </summary>
    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.AddCommand(() => DbSet.ReplaceOneAsync(e => e.Id.Equals(entity.Id), entity, new ReplaceOptions(), cancellationToken));

        return entity;
    }

    /// <summary>
    /// Queues the deletion of a range of entities.
    /// </summary>
    public Task DeleteRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _context.AddCommand(() => DbSet.DeleteOneAsync(e => entities.Any(i => e.Id.Equals(i.Id)), cancellationToken));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Queues the deletion of entities matching the predicate.
    /// </summary>
    public Task DeleteAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        _context.AddCommand(() => DbSet.DeleteOneAsync(predicate, cancellationToken));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Queues the deletion of a specific entity.
    /// </summary>
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.AddCommand(() => DbSet.DeleteOneAsync(e => e.Id.Equals(entity.Id), cancellationToken));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Queues the deletion of an entity by its ID.
    /// </summary>
    public Task DeleteByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        _context.AddCommand(() => DbSet.DeleteOneAsync(e => e.Id.Equals(id), cancellationToken));
        return Task.CompletedTask;
    }
}