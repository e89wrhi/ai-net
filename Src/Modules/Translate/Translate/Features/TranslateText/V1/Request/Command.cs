using AI.Common.Core;
using Translate.Enums;

namespace Translate.Features.TranslateText.V1;

public record TranslateTextCommand(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel, string? ModelId = null) : ICommand<TranslateTextCommandResult>;

public record TranslateTextCommandResult(Guid SessionId, Guid ResultId, string TranslatedText, string ModelId, string? ProviderName);
