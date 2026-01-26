namespace AI.Common.Core;

using global::MassTransit;
using MediatR;

/// <summary>
/// The base interface for all events in our system.
/// It automatically tracks things like when the event happened and what its unique ID is.
/// </summary>
public interface IEvent : INotification
{
    Guid EventId => NewId.NextGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName;
}