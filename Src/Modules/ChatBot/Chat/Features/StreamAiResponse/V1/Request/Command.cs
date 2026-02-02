using MediatR;

namespace ChatBot.Features.StreamAiResponse.V1;

public record StreamAiResponseCommand(Guid SessionId) : IStreamRequest<string>;
