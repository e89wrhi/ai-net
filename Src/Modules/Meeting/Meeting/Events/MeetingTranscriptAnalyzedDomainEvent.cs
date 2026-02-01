using AI.Common.Core;
using Meeting.ValueObjects;

namespace Meeting.Events;

public record MeetingTranscriptAnalyzedDomainEvent(MeetingId Id, TranscriptId TranscriptId, string Transcript): IDomainEvent;