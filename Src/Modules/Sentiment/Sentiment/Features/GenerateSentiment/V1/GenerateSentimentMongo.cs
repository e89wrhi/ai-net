namespace Sentiment.Features.DeleteSentiment.V1;

using Ardalis.GuardClauses;
using Sentiment.Data;
using Sentiment.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using Sentiment.Exceptions;
using System;

public record GenerateSentimentMongo(Guid SentimentId) : InternalCommand;

public class DeleteSentimentMongoHandler : ICommandHandler<GenerateSentimentMongo>
{
    private readonly SentimentReadDbContext _readDbContext;

    public DeleteSentimentMongoHandler(SentimentReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateSentimentMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<TextSentimentReadModel>.Filter.Eq(x => x.Id, request.SentimentId);

        await _readDbContext.Sentiments.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

