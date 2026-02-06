using MediatR;
using Translate.Enums;

namespace Translate.Features.StreamTranslateText.V1;

public record StreamTranslateTextCommand(Guid UserId, string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel, string? ModelId = null) : IStreamRequest<string>;


