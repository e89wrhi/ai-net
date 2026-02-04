namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

public record AnalyzeMeetingTranscriptRequestDto(string Transcript, bool IncludeActionItems, bool IncludeDescisions, string Language, string? ModelId = null);
public record AnalyzeMeetingTranscriptResponseDto(Guid MeetingId, Guid TranscriptId, string Summary, string ModelId, string? ProviderName);
