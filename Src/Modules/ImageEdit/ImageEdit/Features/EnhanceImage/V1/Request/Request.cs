namespace ImageEdit.Features.EnhanceImage.V1;

public record AIEnhanceImageRequestDto(string ImageUrlOrBase64, string Prompt);
public record AIEnhanceImageResponseDto(Guid SessionId, Guid ResultId, string ResultImageUrl);
