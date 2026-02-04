using AI.Common.Core;

namespace Translate.Features.DetectLanguage.V1;

public record DetectLanguageCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<DetectLanguageCommandResult>;

public record DetectLanguageCommandResult(string DetectedLanguageCode, double Confidence, string ModelId, string? ProviderName);
