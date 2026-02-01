namespace ChatBot.Features.StartChat.V1;


using Ardalis.GuardClauses;
using ChatBot.Data;
using ChatBot.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using ChatBot.Exceptions;
using System;

public record StartChatMongo(Guid Id, Guid UserId, string Title, string AiModelId, string SessionStatus, DateTime CreatedAt) : InternalCommand;

public class StartChatMongoHandler : ICommandHandler<StartChatMongo>
{
    private readonly ChatReadDbContext _readDbContext;

    public StartChatMongoHandler(ChatReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(StartChatMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var chat = new ChatSessionReadModel
        {
            Id = request.Id,
            UserId = request.UserId,
            Title = request.Title,
            AiModelId = request.AiModelId,
            SessionStatus = request.SessionStatus,
            LastSentAt = request.CreatedAt,
            Messages = new List<MessageReadModel>()
        };

        await _readDbContext.Chats.InsertOneAsync(chat, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

