namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

public record AnalyzeMeetingTranscriptRequestDto(string Transcript);
public record AnalyzeMeetingTranscriptResponseDto(Guid MeetingId, Guid TranscriptId, string Summary);
