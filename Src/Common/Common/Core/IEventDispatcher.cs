namespace AI.Common.Core;

/// <summary>
/// Interface for an event dispatcher.
/// Defines methods for sending a single event or a list of events asynchronously.
/// Used throughout the application to abstract the mechanism of event distribution.
/// </summary>
public interface IEventDispatcher
{
    public Task SendAsync<T>(IReadOnlyList<T> events, Type type = null, CancellationToken cancellationToken = default)
        where T : IEvent;
    public Task SendAsync<T>(T @event, Type type = null, CancellationToken cancellationToken = default)
        where T : IEvent;
}