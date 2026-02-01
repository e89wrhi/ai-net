using AI.Common.Core;
using TextToSpeech.Enums;
using TextToSpeech.ValueObjects;

namespace TextToSpeech.Events;

public record TextToSpeechSessionFailedDomainEvent(TextToSpeechId Id, TextToSpeechFailureReason Reason): IDomainEvent;
