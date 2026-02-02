namespace ImageCaption.Features.CaptionImage.V1;

public record AIImageCaptionRequestDto(string ImageUrlOrBase64);
public record AIImageCaptionResponseDto(Guid SessionId, Guid ResultId, string Caption);
