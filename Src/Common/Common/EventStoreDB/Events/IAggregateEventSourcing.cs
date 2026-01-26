using AI.Common.Core;

namespace AI.Common.EventStoreDB;

public interface IAggregateEventSourcing : IProjection, IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IDomainEvent[] ClearDomainEvents();
}

public interface IAggregateEventSourcing<T> : IAggregateEventSourcing, IEntity<T>
{
}