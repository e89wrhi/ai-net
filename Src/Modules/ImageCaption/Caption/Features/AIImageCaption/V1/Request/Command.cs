using AI.Common.Core;

namespace ImageCaption.Features.AIImageCaption.V1;

public record AIImageCaptionCommand(string ImageUrlOrBase64) : ICommand<AIImageCaptionCommandResult>;

public record AIImageCaptionCommandResult(Guid SessionId, Guid ResultId, string Caption);

