namespace AI.Common.Core;

/// <summary>
/// An Aggregate is a cluster of domain objects that can be treated as a single unit.
/// This base class specifically helps us track "Domain Events"—things that happened inside the aggregate
/// that other parts of the system might care about.
/// </summary>
public abstract record Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    // A private list to store events until we're ready to "dispatch" them (usually when saving to the DB)
    private readonly List<IDomainEvent> _domainEvents = new();
    
    /// <summary>
    /// Exposes the events as a read-only list so they can't be modified from the outside.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Call this inside your aggregate's methods whenever something significant happens.
    /// Example: AddDomainEvent(new MatchStarted(this.Id));
    /// </summary>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Once the events have been processed, we clear them out so we don't handle them twice.
    /// This is typically called by a Repository or unit of work after a successful save.
    /// </summary>
    public IEvent[] ClearDomainEvents()
    {
        IEvent[] dequeuedEvents = _domainEvents.ToArray();

        _domainEvents.Clear();

        return dequeuedEvents;
    }
}