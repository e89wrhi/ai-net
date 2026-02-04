using AI.Common.Core;

namespace ImageGen.Features.ReGenerateImage.V1;

public record ReGenerateImageCommand(Guid UserId, Guid SessionId, string Instruction, string? ModelId = null) : ICommand<ReGenerateImageCommandResult>;

public record ReGenerateImageCommandResult(Guid ResultId, string ImageUrl, string ModelId, string? ProviderName);

