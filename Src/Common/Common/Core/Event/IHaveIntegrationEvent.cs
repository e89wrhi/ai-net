namespace AI.Common.Core;

/// <summary>
/// A marker interface for domain events that should be automatically wrapped and published as integration events.
/// This simplifies the process of letting the outside world know about internal changes.
/// </summary>
public interface IHaveIntegrationEvent
{
}