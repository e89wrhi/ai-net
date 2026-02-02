using AI.Common.Core;

namespace Translate.Features.DetectLanguage.V1;


public record DetectLanguageCommand(string Text) : ICommand<DetectLanguageCommandResult>;

public record DetectLanguageCommandResult(string DetectedLanguageCode, double Confidence);
