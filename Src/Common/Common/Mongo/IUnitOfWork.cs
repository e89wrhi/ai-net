namespace AI.Common.Mongo;

/// <summary>
/// A Unit of Work pattern interface. 
/// Coordinates the saving of changes across one or more repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Starts a new transaction for the operations within this unit of work.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits all changes made within this unit of work.
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// A generic Unit of Work interface that provides access to the underlying context.
/// </summary>
public interface IUnitOfWork<out TContext> : IUnitOfWork
    where TContext : class
{
    /// <summary>
    /// The database context managed by this unit of work.
    /// </summary>
    TContext Context { get; }
}