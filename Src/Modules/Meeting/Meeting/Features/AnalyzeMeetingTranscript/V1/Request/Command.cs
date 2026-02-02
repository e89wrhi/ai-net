using AI.Common.Core;

namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

public record AnalyzeMeetingTranscriptCommand(string Transcript) : ICommand<AnalyzeMeetingTranscriptCommandResult>;

public record AnalyzeMeetingTranscriptCommandResult(Guid MeetingId, Guid TranscriptId, string Summary);
