namespace ChatBot.Features.DeleteChat.V1;

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

public record DeleteChatMongo(Guid ChatId) : InternalCommand;

public class DeleteChatMongoHandler : ICommandHandler<DeleteChatMongo>
{
    private readonly ChatReadDbContext _readDbContext;

    public DeleteChatMongoHandler(ChatReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(DeleteChatMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ChatSessionReadModel>.Filter.Eq(x => x.Id, request.ChatId);

        await _readDbContext.Chats.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

