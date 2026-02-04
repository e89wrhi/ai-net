using AI.Common.Core;

namespace ImageCaption.Features.AnalyzeImage.V1;

public record AnalyzeImageCommand(Guid UserId, string ImageUrlOrBase64, string? ModelId = null) : ICommand<AnalyzeImageCommandResult>;

public record AnalyzeImageCommandResult(Guid SessionId, Guid ResultId, string Analysis, string ModelId, string? ProviderName);

