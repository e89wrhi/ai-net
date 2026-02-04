using AI.Common.Core;

namespace Meeting.Features.AnalyzeMeetingTranscript.V1;

public record AnalyzeMeetingTranscriptCommand(Guid UserId, string Transcript, bool IncludeActionItems, bool IncludeDescisions, string Language, string? ModelId = null) : ICommand<AnalyzeMeetingTranscriptCommandResult>;

public record AnalyzeMeetingTranscriptCommandResult(Guid MeetingId, Guid TranscriptId, string Summary, string ModelId, string? ProviderName);
