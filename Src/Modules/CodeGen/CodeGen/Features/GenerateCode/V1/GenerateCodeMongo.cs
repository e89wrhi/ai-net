namespace CodeGen.Features.DeleteCodeGen.V1;

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

public record GenerateCodeMongo(Guid CodeGenId) : InternalCommand;

public class DeleteCodeGenMongoHandler : ICommandHandler<GenerateCodeMongo>
{
    private readonly CodeGenReadDbContext _readDbContext;

    public DeleteCodeGenMongoHandler(CodeGenReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateCodeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<CodeGenerationReadModel>.Filter.Eq(x => x.Id, request.CodeGenId);

        await _readDbContext.CodeGens.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

