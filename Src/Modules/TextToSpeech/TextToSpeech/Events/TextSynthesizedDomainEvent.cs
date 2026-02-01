using AI.Common.Core;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Events;

public record TextSynthesizedDomainEvent(TextToSpeechId Id, TextToSpeechResultId ResultId, string Speech) : IDomainEvent;
