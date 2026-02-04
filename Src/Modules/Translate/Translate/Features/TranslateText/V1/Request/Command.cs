using AI.Common.Core;
using Translate.Enums;

namespace Translate.Features.TranslateText.V1;

public record TranslateTextWithAICommand(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel, string? ModelId = null) : ICommand<TranslateTextWithAICommandResult>;

public record TranslateTextWithAICommandResult(Guid SessionId, Guid ResultId, string TranslatedText);
