using AI.Common.Core;

namespace ImageCaption.Features.AnalyzeImage.V1;

public record AnalyzeImageCommand(string ImageUrlOrBase64) : ICommand<AnalyzeImageCommandResult>;

public record AnalyzeImageCommandResult(Guid SessionId, Guid ResultId, string Analysis);

