namespace CodeDebug.Features.DeleteCodeDebug.V1;

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

public record DebugCodeMongo(Guid CodeDebugId) : InternalCommand;

public class DeleteCodeDebugMongoHandler : ICommandHandler<DebugCodeMongo>
{
    private readonly CodeDebugReadDbContext _readDbContext;

    public DeleteCodeDebugMongoHandler(CodeDebugReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(DebugCodeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<CodeDebugSessionReadModel>.Filter.Eq(x => x.Id, request.CodeDebugId);

        await _readDbContext.CodeDebugs.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

