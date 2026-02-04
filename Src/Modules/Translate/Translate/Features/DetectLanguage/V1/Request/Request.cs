namespace Translate.Features.DetectLanguage.V1;

public record DetectLanguageRequestDto(string Text, string? ModelId = null);
public record DetectLanguageResponseDto(string DetectedLanguageCode, double Confidence, string ModelId, string? ProviderName);
