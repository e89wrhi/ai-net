namespace AI.Common.Core;

/// <summary>
/// A marker interface for Aggregate Roots.
/// Every aggregate must expose its domain events so they can be dispatched.
/// </summary>
public interface IAggregate<T> : IAggregate, IEntity<T>
{
}

public interface IAggregate : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IEvent[] ClearDomainEvents();
}