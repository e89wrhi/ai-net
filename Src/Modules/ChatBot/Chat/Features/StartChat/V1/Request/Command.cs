using AI.Common.Core;
using MassTransit;

namespace ChatBot.Features.StartChat.V1;

public record StartChatCommand(Guid UserId, string Title, string AiModelId) : ICommand<StartChatCommandResponse>
{
    public Guid Id { get; init; } = NewId.NextGuid();
}

public record StartChatCommandResponse(Guid Id);
