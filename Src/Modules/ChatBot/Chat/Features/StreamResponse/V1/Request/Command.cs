using MediatR;

namespace ChatBot.Features.StreamResponse.V1;

public record StreamAiResponseCommand(Guid SessionId, string? ModelId = null) : IStreamRequest<string>;

