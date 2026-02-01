using AI.Common.Core;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Events;

public record TextToSpeechSessionCompletedDomainEvent(TextToSpeechId Id): IDomainEvent;
