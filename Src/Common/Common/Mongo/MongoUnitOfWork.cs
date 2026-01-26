namespace AI.Common.Mongo;

/// <summary>
/// A concrete implementation of the MongoDB Unit of Work.
/// It wraps a MongoDbContext and coordinates transaction and save operations.
/// </summary>
public class MongoUnitOfWork<TContext> : IMongoUnitOfWork<TContext>, ITransactionAble
    where TContext : MongoDbContext
{
    public MongoUnitOfWork(TContext context) => Context = context;

    /// <summary>
    /// The database context managed by this unit of work.
    /// </summary>
    public TContext Context { get; }

    /// <summary>
    /// Commits all changes (executes all queued commands) to the database.
    /// </summary>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Starts a new session and transaction.
    /// </summary>
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Rolls back the active transaction.
    /// </summary>
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.RollbackTransaction(cancellationToken);
    }

    /// <summary>
    /// Commits the active transaction.
    /// </summary>
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Context.CommitTransactionAsync(cancellationToken);
    }

    public void Dispose() => Context.Dispose();
}