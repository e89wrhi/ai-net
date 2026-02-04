using AI.Common.Core;

namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

public record AnalyzeMeetingTranscriptCommand(string Transcript, string? ModelId = null) : ICommand<AnalyzeMeetingTranscriptCommandResult>;

public record AnalyzeMeetingTranscriptCommandResult(Guid MeetingId, Guid TranscriptId, string Summary, string ModelId, string? ProviderName);
