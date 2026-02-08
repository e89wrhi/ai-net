using AI.Common.Core;

namespace SimpleMD.Features.SummarizeMD.V1;

public record SummarizeMDCommand(Guid UserId, string Text, string? ModelId = null) : ICommand<SummarizeMDCommandResult>;

public record SummarizeMDCommandResult(string Response, string ModelId, string? ProviderName);
