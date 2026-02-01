namespace CodeGen.Features.SendCodeGen.V1;

using Ardalis.GuardClauses;
using CodeGen.Data;
using CodeGen.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using CodeGen.Exceptions;
using System;

public record ReGenerateCodeMongo(Guid CodeGenId, Guid CodeGenId, string Content, string Sender, int TokenUsed, DateTime SentAt) : InternalCommand;

public class SendCodeGenMongoHandler : ICommandHandler<ReGenerateCodeMongo>
{
    private readonly CodeGenReadDbContext _readDbContext;

    public SendCodeGenMongoHandler(CodeGenReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(ReGenerateCodeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<CodeGenerationReadModel>.Filter.Eq(x => x.Id, request.CodeGenId);
        
        var message = new CodeGenReadModel
        {
            Id = request.CodeGenId,
            Content = request.Content,
            Sender = request.Sender,
            SentAt = request.SentAt,
            TokenUsed = request.TokenUsed
        };

        var update = Builders<CodeGenerationReadModel>.Update
            .Push(x => x.CodeGens, message)
            .Set(x => x.LastSentAt, request.SentAt)
            .Inc(x => x.TotalTokens, request.TokenUsed);

        await _readDbContext.CodeGens.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

