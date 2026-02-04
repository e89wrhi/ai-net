using ImageEdit.Enums;

namespace ImageEdit.Features.RemoveBackground.V1;

public record RemoveBackgroundRequestDto(string ImageUrlOrBase64, ImageEditQuality Quality, ImageFormat Format, string? ModelId = null);
public record RemoveBackgroundResponseDto(Guid SessionId, Guid ResultId, string ResultImageUrl, string ModelId, string? ProviderName);
