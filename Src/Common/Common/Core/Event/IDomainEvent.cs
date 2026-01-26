namespace AI.Common.Core;

/// <summary>
/// Something that happened in the domain that we want other parts of the same domain to know about.
/// Domain events are typically fired from within an Aggregate.
/// </summary>
public interface IDomainEvent : IEvent
{
}