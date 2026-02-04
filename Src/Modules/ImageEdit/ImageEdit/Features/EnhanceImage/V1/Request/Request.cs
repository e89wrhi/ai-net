using ImageEdit.Enums;

namespace ImageEdit.Features.EnhanceImage.V1;

public record EnhanceImageRequestDto(string ImageUrlOrBase64, string Prompt, ImageEditQuality Quality, ImageFormat Format, string? ModelId = null);
public record EnhanceImageResponseDto(Guid SessionId, Guid ResultId, string ResultImageUrl, string ModelId, string? ProviderName);
