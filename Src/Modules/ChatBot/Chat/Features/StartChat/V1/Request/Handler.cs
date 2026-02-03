using ChatBot.Data;
using ChatBot.Models;
using ChatBot.ValueObjects;
using Ardalis.GuardClauses;
using AiOrchestration.Services;
using MassTransit;
using MediatR;

namespace ChatBot.Features.StartChat.V1;

internal class StartChatHandler : IRequestHandler<StartChatCommand, StartChatCommandResponse>
{
    private readonly ChatDbContext _dbContext;

    public StartChatHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StartChatCommandResponse> Handle(StartChatCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chat = ChatSession.Create(
            SessionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Title,
            request.AiModelId);

        await _dbContext.Chats.AddAsync(chat, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StartChatCommandResponse(chat.Id.Value);
    }
}


