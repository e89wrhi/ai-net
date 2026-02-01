namespace CodeDebug.Features.SendCodeDebug.V1;

using Ardalis.GuardClauses;
using CodeDebug.Data;
using CodeDebug.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using CodeDebug.Exceptions;
using System;

public record ReDebugCodeMongo(Guid CodeDebugId, Guid CodeDebugId, string Content, string Sender, int TokenUsed, DateTime SentAt) : InternalCommand;

public class SendCodeDebugMongoHandler : ICommandHandler<ReDebugCodeMongo>
{
    private readonly CodeDebugReadDbContext _readDbContext;

    public SendCodeDebugMongoHandler(CodeDebugReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(ReDebugCodeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<CodeDebugSessionReadModel>.Filter.Eq(x => x.Id, request.CodeDebugId);
        
        var message = new CodeDebugReadModel
        {
            Id = request.CodeDebugId,
            Content = request.Content,
            Sender = request.Sender,
            SentAt = request.SentAt,
            TokenUsed = request.TokenUsed
        };

        var update = Builders<CodeDebugSessionReadModel>.Update
            .Push(x => x.CodeDebugs, message)
            .Set(x => x.LastSentAt, request.SentAt)
            .Inc(x => x.TotalTokens, request.TokenUsed);

        await _readDbContext.CodeDebugs.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

