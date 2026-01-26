
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AI.Common.PersistMessageProcessor;
using AI.Common.Web;
using System.Security.Claims;

namespace AI.Common.Core;

/// <summary>
/// The EventDispatcher is the central hub for handling what happens after a domain operation.
/// It takes domain events (things that just happened) and decides where they need to go next—
/// whether that's becoming an integration event for other modules or an internal command for later background processing.
/// </summary>
public sealed class EventDispatcher(
    IServiceScopeFactory serviceScopeFactory,
    IEventMapper eventMapper,
    ILogger<EventDispatcher> logger,
    IPersistMessageProcessor persistMessageProcessor,
    IHttpContextAccessor httpContextAccessor
)
    : IEventDispatcher
{
    /// <summary>
    /// Sends a list of events to their respective destinations.
    /// This is where the magic happens: we figure out if we're dealing with domain events or integration events
    /// and route them accordingly.
    /// </summary>
    public async Task SendAsync<T>(IReadOnlyList<T> events, Type type = null,
                                   CancellationToken cancellationToken = default)
        where T : IEvent
    {
        if (events.Count > 0)
        {
            // We determine if this is a standard domain event or a "hidden" internal command
            var eventType = type != null && type.IsAssignableTo(typeof(IInternalCommand))
                ? EventType.InternalCommand
                : EventType.DomainEvent;

            // Helper to push integration events into our persistent message processor (Outbox)
            async Task PublishIntegrationEvent(IReadOnlyList<IIntegrationEvent> integrationEvents)
            {
                foreach (var integrationEvent in integrationEvents)
                {
                    await persistMessageProcessor.PublishMessageAsync(
                        new MessageEnvelope(integrationEvent, SetHeaders()),
                        cancellationToken);
                }
            }

            // Route based on what kind of events we've been handed
            switch (events)
            {
                case IReadOnlyList<IDomainEvent> domainEvents:
                    {
                        // Domain events usually need to be "translated" to integration events
                        // so we don't leak internal domain details to the outside world.
                        var integrationEvents = await MapDomainEventToIntegrationEventAsync(domainEvents)
                        .ConfigureAwait(false);

                        await PublishIntegrationEvent(integrationEvents);
                        break;
                    }

                case IReadOnlyList<IIntegrationEvent> integrationEvents:
                    // If they are already integration events, just publish them!
                    await PublishIntegrationEvent(integrationEvents);
                    break;
            }

            // If we've got an internal command, we save it for background processing
            if (type != null && eventType == EventType.InternalCommand)
            {
                var internalMessages = await MapDomainEventToInternalCommandAsync(events as IReadOnlyList<IDomainEvent>)
                    .ConfigureAwait(false);

                foreach (var internalMessage in internalMessages)
                {
                    await persistMessageProcessor.AddInternalMessageAsync(internalMessage, cancellationToken);
                }
            }
        }
    }

    public async Task SendAsync<T>(T @event, Type type = null,
        CancellationToken cancellationToken = default)
        where T : IEvent =>
        await SendAsync(new[] { @event }, type, cancellationToken);


    /// <summary>
    /// Converts domain events into integration events using the registered IEventMapper.
    /// This keeps our domain pure and our external contracts stable.
    /// </summary>
    private Task<IReadOnlyList<IIntegrationEvent>> MapDomainEventToIntegrationEventAsync(
        IReadOnlyList<IDomainEvent> events)
    {
        logger.LogTrace("Processing integration events start...");

        // First, check if any events already "carry" their integration counterpart
        var wrappedIntegrationEvents = GetWrappedIntegrationEvents(events.ToList())?.ToList();
        if (wrappedIntegrationEvents?.Count > 0)
            return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(wrappedIntegrationEvents);

        var integrationEvents = new List<IIntegrationEvent>();
        using var scope = serviceScopeFactory.CreateScope();
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            logger.LogTrace($"Handling domain event: {eventType.Name}");

            // Ask the mapper: "What should the rest of the world know about this?"
            var integrationEvent = eventMapper.MapToIntegrationEvent(@event);

            if (integrationEvent is null)
                continue;

            integrationEvents.Add(integrationEvent);
        }

        logger.LogTrace("Processing integration events done...");

        return Task.FromResult<IReadOnlyList<IIntegrationEvent>>(integrationEvents);
    }


    /// <summary>
    /// Converts domain events to internal commands.
    /// Useful for "fire and forget" logic that should happen within the same module but asynchronously.
    /// </summary>
    private Task<IReadOnlyList<IInternalCommand>> MapDomainEventToInternalCommandAsync(
        IReadOnlyList<IDomainEvent> events)
    {
        logger.LogTrace("Processing internal message start...");

        var internalCommands = new List<IInternalCommand>();
        using var scope = serviceScopeFactory.CreateScope();
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            logger.LogTrace($"Handling domain event: {eventType.Name}");

            var integrationEvent = eventMapper.MapToInternalCommand(@event);

            if (integrationEvent is null)
                continue;

            internalCommands.Add(integrationEvent);
        }

        logger.LogTrace("Processing internal message done...");

        return Task.FromResult<IReadOnlyList<IInternalCommand>>(internalCommands);
    }

    /// <summary>
    /// Some domain events implement IHaveIntegrationEvent, meaning they know how to wrap themselves.
    /// This method extracts those pre-wrapped events.
    /// </summary>
    private IEnumerable<IIntegrationEvent> GetWrappedIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents.Where(x =>
                     x is IHaveIntegrationEvent))
        {
            var genericType = typeof(IntegrationEventWrapper<>)
                .MakeGenericType(domainEvent.GetType());

            var domainNotificationEvent = (IIntegrationEvent)Activator
                .CreateInstance(genericType, domainEvent);

            yield return domainNotificationEvent;
        }
    }

    /// <summary>
    /// Attaches standard metadata (CorrelationId, UserId) to every outgoing message.
    /// This is crucial for tracing and knowing "who did what" across the system.
    /// </summary>
    private IDictionary<string, object> SetHeaders()
    {
        var headers = new Dictionary<string, object>();
        headers.Add("CorrelationId", httpContextAccessor?.HttpContext?.GetCorrelationId());
        headers.Add("UserId", httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        headers.Add("UserName", httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Name));

        return headers;
    }
}
