using AI.Common.Core;
using MassTransit;

namespace ChatBot.Features.SendMessage.V1;

public record SendMessageCommand(Guid SessionId, string Content) : ICommand<SendMessageCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record SendMessageCommandResponse(Guid MessageId);
