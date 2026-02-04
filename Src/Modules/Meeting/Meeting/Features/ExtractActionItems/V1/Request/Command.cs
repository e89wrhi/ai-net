using AI.Common.Core;

namespace Meeting.Features.ExtractActionItems.V1;

public record ExtractActionItemsCommand(Guid UserId, string Transcript, bool IncludeActionItems, bool IncludeDescisions, string Language, string? ModelId = null) : ICommand<ExtractActionItemsCommandResult>;

public record ExtractActionItemsCommandResult(Guid MeetingId, string ActionItems, string ModelId, string? ProviderName);
