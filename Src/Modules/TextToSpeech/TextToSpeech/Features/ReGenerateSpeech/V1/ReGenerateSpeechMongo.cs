namespace TextToSpeech.Features.SendTextToSpeech.V1;

using Ardalis.GuardClauses;
using TextToSpeech.Data;
using TextToSpeech.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using TextToSpeech.Exceptions;
using System;

public record ReGenerateSpeechMongo(Guid TextToSpeechId, Guid TextToSpeechId, string Content, string Sender, int TokenUsed, DateTime SentAt) : InternalCommand;

public class SendTextToSpeechMongoHandler : ICommandHandler<ReGenerateSpeechMongo>
{
    private readonly TextToSpeechReadDbContext _readDbContext;

    public SendTextToSpeechMongoHandler(TextToSpeechReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(ReGenerateSpeechMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<TextToSpeechSessionReadModel>.Filter.Eq(x => x.Id, request.TextToSpeechId);
        
        var message = new TextToSpeechReadModel
        {
            Id = request.TextToSpeechId,
            Content = request.Content,
            Sender = request.Sender,
            SentAt = request.SentAt,
            TokenUsed = request.TokenUsed
        };

        var update = Builders<TextToSpeechSessionReadModel>.Update
            .Push(x => x.TextToSpeechs, message)
            .Set(x => x.LastSentAt, request.SentAt)
            .Inc(x => x.TotalTokens, request.TokenUsed);

        await _readDbContext.TextToSpeechs.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

