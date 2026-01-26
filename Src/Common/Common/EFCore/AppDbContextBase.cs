using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using AI.Common.Core;
using AI.Common.Web;
using System.Collections.Immutable;
using System.Data;

namespace AI.Common.EFCore;

/// <summary>
/// This is the foundation for all our database contexts.
/// It doesn't just talk to the database; it's smart enough to handle transactions, 
/// automatic auditing (who changed what and when), and even soft deletes.
/// </summary>
public abstract class AppDbContextBase : DbContext, IDbContext
{
    private readonly ICurrentUserProvider? _currentUserProvider;
    private readonly ILogger<AppDbContextBase>? _logger;
    private IDbContextTransaction _currentTransaction;

    protected AppDbContextBase(DbContextOptions options, ICurrentUserProvider? currentUserProvider = null, ILogger<AppDbContextBase>? logger = null) :
        base(options)
    {
        _currentUserProvider = currentUserProvider;
        _logger = logger;
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
    }

    /// <summary>
    /// Creates a strategy for handling transient failures (like a quick network blip).
    /// </summary>
    public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
            return;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    /// <summary>
    /// Saves everything to the database and then commits the transaction.
    /// If anything fails, it rolls back automatically to keep our data consistent.
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await _currentTransaction?.CommitAsync(cancellationToken)!;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }
    }


    /// <summary>
    /// A helper to run a block of code inside a resilient transaction.
    /// If the connection drops halfway, EF will retry the whole block for us.
    /// </summary>
    public Task ExecuteTransactionalAsync(CancellationToken cancellationToken = default)
    {
        var strategy = CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    /// <summary>
    /// Our custom SaveChanges that injects auditing logic and handles concurrency "collisions".
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Update audit fields (CreatedAt, ModifiedBy, etc.)
        OnBeforeSaving();
        
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        // 2. Handle cases where two users edit the same record at the exact same time
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

                if (databaseValues == null)
                {
                    _logger.LogError("The record no longer exists in the database, The record has been deleted by another user.");
                    throw;
                }

                // We basically say: "I see someone else changed it, let's accept their baseline and try again."
                entry.OriginalValues.SetValues(databaseValues);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Scans all the objects we're about to save and pulls out any Domain Events they've raised.
    /// This is how we trigger background tasks or notify other modules.
    /// </summary>
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<IAggregate>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToImmutableList();

        // Clear them out so they don't get fired again if we save again in the same request.
        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        return domainEvents.ToImmutableList();
    }

    /// <summary>
    /// The "Magic" behind our auditing and soft deletes.
    /// It looks at what's changing and fills in the blanks for us.
    /// </summary>
    private void OnBeforeSaving()
    {
        try
        {
            foreach (var entry in ChangeTracker.Entries<IAggregate>())
            {
                var isAuditable = entry.Entity.GetType().IsAssignableTo(typeof(IAggregate));
                var userId = _currentUserProvider?.GetCurrentUserId() ?? 0;

                if (isAuditable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedBy = userId;
                            entry.Entity.CreatedAt = DateTime.Now;
                            break;

                        case EntityState.Modified:
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.Version++;
                            break;

                        case EntityState.Deleted:
                            // Soft Delete: Instead of deleting the row, we just mark it as deleted.
                            entry.State = EntityState.Modified;
                            entry.Entity.LastModifiedBy = userId;
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.IsDeleted = true;
                            entry.Entity.Version++;
                            break;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("An error occurred while trying to process aggregate audit fields.", ex);
        }
    }
}
