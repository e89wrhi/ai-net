namespace AI.Common.Mongo;

/// <summary>
/// Interface for objects that support transactional operations.
/// </summary>
public interface ITransactionAble
{
    /// <summary>
    /// Starts a new transaction.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the active transaction.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the active transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
}