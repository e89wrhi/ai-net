using MediatR;

namespace AutoComplete.Features.StreamAICompletion.V1;


public record StreamAICompletionCommand(Guid UserId, string Prompt) : IStreamRequest<string>;
