namespace ImageEdit.Features.RemoveBackground.V1;

public record RemoveBackgroundRequestDto(string ImageUrlOrBase64, string? ModelId = null);
public record RemoveBackgroundResponseDto(Guid SessionId, Guid ResultId, string ResultImageUrl, string ModelId, string? ProviderName);
