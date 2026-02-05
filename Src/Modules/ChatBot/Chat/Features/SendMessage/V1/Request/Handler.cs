using AiOrchestration.ValueObjects;
using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Enums;
using ChatBot.Exceptions;
using ChatBot.Models;
using ChatBot.ValueObjects;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Features.SendMessage.V1;

internal class SendMessageHandler : IRequestHandler<SendMessageCommand, SendMessageCommandResponse>
{
    private readonly ChatDbContext _dbContext;

    public SendMessageHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SendMessageCommandResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NullOrWhiteSpace(request.Content, nameof(request.Content));

        var chat = await _dbContext.Chats
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == SessionId.Of(request.SessionId), cancellationToken);

        if (chat == null)
        {
            throw new ChatNotFoundException(request.SessionId);
        }

        var sequence = chat.Messages.Count + 1;
        
        // Estimate tokens for user message (rough estimate: 1 token ≈ 4 characters)
        var estimatedTokens = request.Content.Length / 4;
        
        var message = ChatMessage.Create(
            MessageId.Of(NewId.NextGuid()),
            chat.Id,
            MessageSender.User,
            MessageContent.Of(request.Content),
            TokenCount.Of(estimatedTokens),
            CostEstimate.Of(0),
            sequence);

        chat.AddMessage(message);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SendMessageCommandResponse(message.Id.Value);
    }
}

