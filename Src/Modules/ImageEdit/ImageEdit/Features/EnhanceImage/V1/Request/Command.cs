using AI.Common.Core;
using ImageEdit.Enums;

namespace ImageEdit.Features.EnhanceImage.V1;

public record AIEnhanceImageCommand(Guid UserId, string ImageUrlOrBase64, string Prompt, ImageEditQuality Quality, ImageFormat Format, string? ModelId = null) : ICommand<AIEnhanceImageCommandResult>;

public record AIEnhanceImageCommandResult(Guid SessionId, Guid ResultId, string ResultImageUrl, string ModelId, string? ProviderName);

