using ImageGen.Enums;

namespace ImageGen.Features.GenerateImage.V1;


public record GenerateImageRequestDto(string Prompt, ImageSize Size, ImageStyle Style);
public record GenerateImageResponseDto(Guid SessionId, Guid ResultId, string ImageUrl);
