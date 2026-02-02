namespace ImageCaption.Features.AnalyzeImage.V1;

public record AnalyzeImageRequestDto(string ImageUrlOrBase64);
public record AnalyzeImageResponseDto(Guid SessionId, Guid ResultId, string Analysis);
