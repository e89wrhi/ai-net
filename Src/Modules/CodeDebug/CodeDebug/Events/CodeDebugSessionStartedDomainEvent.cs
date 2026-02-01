using AI.Common.Core;
using AiOrchestration.ValueObjects;
using CodeDebug.ValueObjects;

namespace CodeDebug.Events;

public record CodeDebugSessionStartedDomainEvent(CodeDebugId Id, UserId UserId, ModelId AiModelId) : IDomainEvent;
