using AI.Common.Core;
using AiOrchestration.ValueObjects;
using SpeechToText.ValueObjects;

namespace SpeechToText.Events;

public record SpeechToTextSessionStartedDomainEvent(SpeechToTextId Id, UserId UserId, ModelId AiModelId) : IDomainEvent;
