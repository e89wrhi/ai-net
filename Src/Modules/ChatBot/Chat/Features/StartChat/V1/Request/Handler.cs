using AiOrchestration.ValueObjects;
using ChatBot.Data;
using ChatBot.Models;
using ChatBot.ValueObjects;
using Ardalis.GuardClauses;
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
        Guard.Against.NullOrWhiteSpace(request.Title, nameof(request.Title));
        Guard.Against.NullOrWhiteSpace(request.AiModelId, nameof(request.AiModelId));

        // Create default configuration
        var configuration = new ChatConfiguration(
            Temperature.Of(0.7f),
            TokenCount.Of(4096), // Max tokens
            SystemPrompt.Of("You are a helpful AI assistant.")
        );

        var chat = ChatSession.Create(
            SessionId.Of(NewId.NextGuid()),
            UserId.Of(request.UserId),
            request.Title,
            string.Empty,
            ModelId.Of(request.AiModelId),
            configuration);

        await _dbContext.Chats.AddAsync(chat, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new StartChatCommandResponse(chat.Id.Value);
    }
}


