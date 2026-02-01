using AI.Common.Core;
using SpeechToText.ValueObjects;

namespace SpeechToText.Events;

public record SpeechTranscribedDomainEvent(SpeechToTextId Id, SpeechToTextResultId ResultId, string Transcript) : IDomainEvent;
