namespace ChatBot.Features.SendMessage.V1;

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

public record SendMessageMongo(Guid ChatId, Guid MessageId, string Content, string Sender, int TokenUsed, DateTime SentAt) : InternalCommand;

public class SendMessageMongoHandler : ICommandHandler<SendMessageMongo>
{
    private readonly ChatReadDbContext _readDbContext;

    public SendMessageMongoHandler(ChatReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(SendMessageMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ChatSessionReadModel>.Filter.Eq(x => x.Id, request.ChatId);
        
        var message = new MessageReadModel
        {
            Id = request.MessageId,
            Content = request.Content,
            Sender = request.Sender,
            SentAt = request.SentAt,
            TokenUsed = request.TokenUsed
        };

        var update = Builders<ChatSessionReadModel>.Update
            .Push(x => x.Messages, message)
            .Set(x => x.LastSentAt, request.SentAt)
            .Inc(x => x.TotalTokens, request.TokenUsed);

        await _readDbContext.Chats.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

