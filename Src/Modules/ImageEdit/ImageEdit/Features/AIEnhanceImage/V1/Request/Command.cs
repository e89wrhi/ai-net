using AI.Common.Core;

namespace ImageEdit.Features.AiEnhanceImage.V1;

public record AIEnhanceImageCommand(string ImageUrlOrBase64, string Prompt) : ICommand<AIEnhanceImageCommandResult>;

public record AIEnhanceImageCommandResult(Guid SessionId, Guid ResultId, string ResultImageUrl);

