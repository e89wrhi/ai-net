namespace ImageGen.Features.ReGenerateImage.V1;


public record ReGenerateImageRequestDto(Guid SessionId, string Instruction);
public record ReGenerateImageResponseDto(Guid ResultId, string ImageUrl);
