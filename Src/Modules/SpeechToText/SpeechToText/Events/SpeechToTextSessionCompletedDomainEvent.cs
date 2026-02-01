using AI.Common.Core;
using SpeechToText.ValueObjects;

namespace SpeechToText.Events;

public record SpeechToTextSessionCompletedDomainEvent(SpeechToTextId Id) : IDomainEvent;
