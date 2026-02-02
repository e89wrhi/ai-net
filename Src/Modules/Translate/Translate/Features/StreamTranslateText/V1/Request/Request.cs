using Translate.Enums;

namespace Translate.Features.StreamTranslateText.V1;

public record StreamTranslateTextRequestDto(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel);
