using MongoDB.Driver;

namespace AI.Common.Mongo;

/// <summary>
/// An abstraction for our MongoDB context.
/// It provides access to collections and handles transactions in a way that feels similar to Entity Framework.
/// </summary>
public interface IMongoDbContext : IDisposable
{
    /// <summary>
    /// Gets a MongoDB collection for the specified type.
    /// </summary>
    IMongoCollection<T> GetCollection<T>(string? name = null);

    /// <summary>
    /// Executes all cached commands within a single unit of work (and transaction if active).
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a new session and transaction.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the active transaction and ends the session.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Aborts the active transaction and ends the session.
    /// </summary>
    Task RollbackTransaction(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a command (e.g., an insert or update) to the execution queue.
    /// Commands are executed when SaveChangesAsync is called.
    /// </summary>
    void AddCommand(Func<Task> func);
}