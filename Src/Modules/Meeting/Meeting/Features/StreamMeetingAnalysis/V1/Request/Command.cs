using MediatR;

namespace Meeting.Features.StreamMeetingAnalysis.V1;


public record StreamMeetingAnalysisCommand(string Transcript) : IStreamRequest<string>;
