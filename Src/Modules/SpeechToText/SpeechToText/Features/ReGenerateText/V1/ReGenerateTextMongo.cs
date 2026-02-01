namespace SpeechToText.Features.SendSpeechToText.V1;

using Ardalis.GuardClauses;
using SpeechToText.Data;
using SpeechToText.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using SpeechToText.Exceptions;
using System;

public record ReGenerateTextMongo(Guid SpeechToTextId, Guid SpeechToTextId, string Content, string Sender, int TokenUsed, DateTime SentAt) : InternalCommand;

public class SendSpeechToTextMongoHandler : ICommandHandler<ReGenerateTextMongo>
{
    private readonly SpeechToTextReadDbContext _readDbContext;

    public SendSpeechToTextMongoHandler(SpeechToTextReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(ReGenerateTextMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<SpeechToTextSessionReadModel>.Filter.Eq(x => x.Id, request.SpeechToTextId);
        
        var message = new SpeechToTextReadModel
        {
            Id = request.SpeechToTextId,
            Content = request.Content,
            Sender = request.Sender,
            SentAt = request.SentAt,
            TokenUsed = request.TokenUsed
        };

        var update = Builders<SpeechToTextSessionReadModel>.Update
            .Push(x => x.SpeechToTexts, message)
            .Set(x => x.LastSentAt, request.SentAt)
            .Inc(x => x.TotalTokens, request.TokenUsed);

        await _readDbContext.SpeechToTexts.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

