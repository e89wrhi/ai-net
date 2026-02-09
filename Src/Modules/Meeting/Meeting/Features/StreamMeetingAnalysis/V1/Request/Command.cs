using MediatR;

namespace Meeting.Features.StreamMeetingAnalysis.V1;

public record StreamMeetingAnalysisCommand(Guid UserId, string Transcript, bool IncludeActionItems, bool IncludeDescisions, string Language, string? ModelId = null) : IStreamRequest<string>;


