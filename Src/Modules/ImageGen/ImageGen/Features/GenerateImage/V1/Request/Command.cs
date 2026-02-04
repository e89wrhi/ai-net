using AI.Common.Core;
using ImageGen.Enums;

namespace ImageGen.Features.GenerateImage.V1;

public record GenerateImageCommand(string Prompt, ImageSize Size, ImageStyle Style, string? ModelId = null) : ICommand<GenerateImageCommandResult>;

public record GenerateImageCommandResult(Guid SessionId, Guid ResultId, string ImageUrl, string ModelId, string? ProviderName);
