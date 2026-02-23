using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record AutoCompleteRequested(Guid Id, string Context) : IIntegrationEvent;
public record SuggestionSelected(Guid Id, string Suggestion) : IIntegrationEvent;
public record ModelFineTuningStarted(Guid Id) : IIntegrationEvent;
public record ContextCacheUpdated(Guid Id) : IIntegrationEvent;
