namespace Summary.Features.DeleteSummary.V1;

using Ardalis.GuardClauses;
using Summary.Data;
using Summary.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using Summary.Exceptions;
using System;

public record SummarizeTextMongo(Guid SummaryId) : InternalCommand;

public class DeleteSummaryMongoHandler : ICommandHandler<SummarizeTextMongo>
{
    private readonly SummaryReadDbContext _readDbContext;

    public DeleteSummaryMongoHandler(SummaryReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(SummarizeTextMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<TextSummaryReadModel>.Filter.Eq(x => x.Id, request.SummaryId);

        await _readDbContext.Summarys.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

