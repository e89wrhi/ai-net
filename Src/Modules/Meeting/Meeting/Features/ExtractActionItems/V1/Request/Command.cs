using AI.Common.Core;

namespace Meeting.Features.ExtractActionItems.V1;

public record ExtractActionItemsCommand(string Transcript) : ICommand<ExtractActionItemsCommandResult>;

public record ExtractActionItemsCommandResult(Guid MeetingId, string ActionItems);
