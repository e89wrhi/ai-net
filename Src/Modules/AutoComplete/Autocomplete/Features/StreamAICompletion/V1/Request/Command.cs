using MediatR;

namespace AutoComplete.Features.StreamAICompletion.V1;

public record StreamAutoCompleteCommand(Guid UserId, string Prompt) : IStreamRequest<string>;
