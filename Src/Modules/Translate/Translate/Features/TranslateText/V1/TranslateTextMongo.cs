namespace Translate.Features.DeleteTranslate.V1;

using Ardalis.GuardClauses;
using Translate.Data;
using Translate.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using Translate.Exceptions;
using System;

public record TranslateTextMongo(Guid TranslateId) : InternalCommand;

public class DeleteTranslateMongoHandler : ICommandHandler<TranslateTextMongo>
{
    private readonly TranslateReadDbContext _readDbContext;

    public DeleteTranslateMongoHandler(TranslateReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(TranslateTextMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<TranslationSessionReadModel>.Filter.Eq(x => x.Id, request.TranslateId);

        await _readDbContext.Translates.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

