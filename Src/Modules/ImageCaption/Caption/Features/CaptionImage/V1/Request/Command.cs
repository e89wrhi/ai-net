using AI.Common.Core;

namespace ImageCaption.Features.CaptionImage.V1;

public record ImageCaptionCommand(Guid UserId, string ImageUrlOrBase64, string? ModelId = null) : ICommand<ImageCaptionCommandResult>;

public record ImageCaptionCommandResult(Guid SessionId, Guid ResultId, string Caption, string ModelId, string? ProviderName);

