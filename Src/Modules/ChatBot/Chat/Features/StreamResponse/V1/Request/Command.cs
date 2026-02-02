using MediatR;

namespace ChatBot.Features.StreamResponse.V1;

public record StreamAiResponseCommand(Guid SessionId) : IStreamRequest<string>;
