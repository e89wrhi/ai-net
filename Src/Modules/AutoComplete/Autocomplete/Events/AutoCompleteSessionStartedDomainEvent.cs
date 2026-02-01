using AI.Common.Core;
using AiOrchestration.ValueObjects;
using AutoComplete.ValueObjects;

namespace AutoComplete.Events;

public record AutoCompleteSessionStartedDomainEvent(AutoCompleteId Id, UserId UserId, ModelId AiModelId) : IDomainEvent;
