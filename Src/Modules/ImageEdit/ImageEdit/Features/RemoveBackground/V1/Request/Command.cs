using AI.Common.Core;

namespace ImageEdit.Features.RemoveBackground.V1;

public record RemoveBackgroundCommand(Guid UserId, string ImageUrlOrBase64, string? ModelId = null) : ICommand<RemoveBackgroundCommandResult>;

public record RemoveBackgroundCommandResult(Guid SessionId, Guid ResultId, string ResultImageUrl, string ModelId, string? ProviderName);
