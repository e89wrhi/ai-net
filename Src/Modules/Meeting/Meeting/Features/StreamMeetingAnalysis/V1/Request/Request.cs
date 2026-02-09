namespace Meeting.Features.StreamMeetingAnalysis.V1;

public record StreamMeetingAnalysisRequestDto(Guid UserId, string Transcript, bool IncludeActionItems, bool IncludeDescisions, string Language, string? ModelId = null);

