using Translate.Enums;

namespace Translate.Features.TranslateText.V1;

public record TranslateTextWithAIRequestDto(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel);
public record TranslateTextWithAIResponseDto(Guid SessionId, Guid ResultId, string TranslatedText);
