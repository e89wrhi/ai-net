using ImageCaption.Enums;

namespace ImageCaption.Features.AnalyzeImage.V1;

public record AnalyzeImageRequestDto(string ImageUrlOrBase64, CaptionDetailLevel Level, string? ModelId = null);
public record AnalyzeImageResponseDto(Guid SessionId, Guid ResultId, string Analysis, string ModelId, string? ProviderName);
