using Translate.Enums;

namespace Translate.Features.TranslateText.V1;

public record TranslateTextRequestDto(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel, string? ModelId = null);
public record TranslateTextResponseDto(Guid SessionId, Guid ResultId, string TranslatedText, string ModelId, string? ProviderName);
