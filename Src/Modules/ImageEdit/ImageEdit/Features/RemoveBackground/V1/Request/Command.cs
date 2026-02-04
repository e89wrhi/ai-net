using AI.Common.Core;
using ImageEdit.Enums;

namespace ImageEdit.Features.RemoveBackground.V1;

public record RemoveBackgroundCommand(Guid UserId, string ImageUrlOrBase64, ImageEditQuality Quality, ImageFormat Format, string? ModelId = null) : ICommand<RemoveBackgroundCommandResult>;

public record RemoveBackgroundCommandResult(Guid SessionId, Guid ResultId, string ResultImageUrl, string ModelId, string? ProviderName);
