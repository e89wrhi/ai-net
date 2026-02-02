namespace Translate.Features.DetectLanguage.V1;


public record DetectLanguageRequestDto(string Text);
public record DetectLanguageResponseDto(string DetectedLanguageCode, double Confidence);
