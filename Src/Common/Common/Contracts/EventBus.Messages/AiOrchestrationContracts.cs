using AI.Common.Core;

namespace AI.Contracts.EventBus.Messages;

public record OrchestrationStarted(Guid Id, string WorkflowName) : IIntegrationEvent;
public record OrchestrationCompleted(Guid Id) : IIntegrationEvent;
public record OrchestrationFailed(Guid Id, string Reason) : IIntegrationEvent;
public record NodeExecuted(Guid Id, string NodeId, string Result) : IIntegrationEvent;
