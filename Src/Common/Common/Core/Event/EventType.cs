namespace AI.Common.Core;

/// <summary>
/// Categorizes the different types of events in our system.
/// This helps the EventDispatcher decide how to route each message.
/// </summary>
[Flags]
public enum EventType
{
    DomainEvent = 1,
    IntegrationEvent = 2,
    InternalCommand = 4
}