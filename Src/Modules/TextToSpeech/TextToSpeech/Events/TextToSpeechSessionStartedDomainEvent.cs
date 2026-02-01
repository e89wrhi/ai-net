using AI.Common.Core;
using AiOrchestration.ValueObjects;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Events;

public record TextToSpeechSessionStartedDomainEvent(TextToSpeechId Id, UserId UserId, ModelId AiModelId) : IDomainEvent;
