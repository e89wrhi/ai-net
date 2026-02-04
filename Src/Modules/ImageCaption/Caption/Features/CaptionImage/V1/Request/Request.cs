using ImageCaption.Enums;

namespace ImageCaption.Features.CaptionImage.V1;

public record ImageCaptionRequestDto(string ImageUrlOrBase64, CaptionDetailLevel Level, string? ModelId = null);
public record ImageCaptionResponseDto(Guid SessionId, Guid ResultId, string Caption, string ModelId, string? ProviderName);
