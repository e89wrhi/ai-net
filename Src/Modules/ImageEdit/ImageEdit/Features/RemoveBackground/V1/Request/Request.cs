namespace ImageEdit.Features.RemoveBackground.V1;

public record RemoveBackgroundRequestDto(string ImageUrlOrBase64);
public record RemoveBackgroundResponseDto(Guid SessionId, Guid ResultId, string ResultImageUrl);

