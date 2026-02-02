using MediatR;
using Translate.Enums;

namespace Translate.Features.StreamTranslateText.V1;


public record StreamTranslateTextCommand(string Text, string SourceLanguage, string TargetLanguage, TranslationDetailLevel DetailLevel) : IStreamRequest<string>;
