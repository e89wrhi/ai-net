using AI.Common.Core;

namespace AI.Common.Contracts.EventBus.Messages;

public record MatchCreated(Guid Id) : IIntegrationEvent;
public record MatchUpdated(Guid Id) : IIntegrationEvent;
public record MatchDeleted(Guid Id) : IIntegrationEvent;
public record MatchScoreUpdated(Guid Id) : IIntegrationEvent;