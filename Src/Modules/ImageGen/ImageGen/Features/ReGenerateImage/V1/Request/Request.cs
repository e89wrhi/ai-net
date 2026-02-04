namespace ImageGen.Features.ReGenerateImage.V1;

public record ReGenerateImageRequestDto(Guid SessionId, string Instruction, string? ModelId = null);
public record ReGenerateImageResponseDto(Guid ResultId, string ImageUrl, string ModelId, string? ProviderName);
