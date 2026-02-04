using ImageGen.Enums;

namespace ImageGen.Features.GenerateImage.V1;

public record GenerateImageRequestDto(string Prompt, ImageSize Size, ImageStyle Style, ImageQuality Quality, string? ModelId = null);
public record GenerateImageResponseDto(Guid SessionId, Guid ResultId, string ImageUrl, string ModelId, string? ProviderName);
