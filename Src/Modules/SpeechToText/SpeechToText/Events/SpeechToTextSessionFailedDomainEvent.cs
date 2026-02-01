using AI.Common.Core;
using SpeechToText.Enums;
using SpeechToText.ValueObjects;

namespace SpeechToText.Events;

public record SpeechToTextSessionFailedDomainEvent(SpeechToTextId Id, SpeechToTextFailureReason Reason): IDomainEvent;
