using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record DebugSessionStarted(Guid Id, string Language) : IIntegrationEvent;
public record BugFound(Guid Id, string Description, string Severity) : IIntegrationEvent;
public record FixSuggested(Guid Id, string Suggestion) : IIntegrationEvent;
public record UnitTestsGenerated(Guid Id, int Count) : IIntegrationEvent;
