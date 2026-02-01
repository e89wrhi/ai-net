using AI.Common.Core;
using AiOrchestration.ValueObjects;
using ChatBot.ValueObjects;

namespace ChatBot.Events;

public record ChatSessionStartedDomainEvent(SessionId Id, UserId UserId, string Title, ModelId AiModelId) : IDomainEvent;
