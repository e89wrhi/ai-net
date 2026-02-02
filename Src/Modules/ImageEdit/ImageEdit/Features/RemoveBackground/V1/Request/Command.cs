using AI.Common.Core;

namespace ImageEdit.Features.RemoveBackground.V1;

public record RemoveBackgroundCommand(string ImageUrlOrBase64) : ICommand<RemoveBackgroundCommandResult>;

public record RemoveBackgroundCommandResult(Guid SessionId, Guid ResultId, string ResultImageUrl);
