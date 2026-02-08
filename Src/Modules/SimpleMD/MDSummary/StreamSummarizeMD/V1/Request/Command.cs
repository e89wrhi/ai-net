using AI.Common.Core;

namespace SimpleMD.Features.StreamSummarizeMD.V1;

public record StreamSummarizeMDCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<StreamSummarizeMDCommandResult>;

public record StreamSummarizeMDCommandResult(string Response, string ModelId, string? ProviderName);
