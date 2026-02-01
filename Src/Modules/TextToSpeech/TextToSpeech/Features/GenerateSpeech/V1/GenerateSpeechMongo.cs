namespace TextToSpeech.Features.DeleteTextToSpeech.V1;

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

public record GenerateSpeechMongo(Guid TextToSpeechId) : InternalCommand;

public class DeleteTextToSpeechMongoHandler : ICommandHandler<GenerateSpeechMongo>
{
    private readonly TextToSpeechReadDbContext _readDbContext;

    public DeleteTextToSpeechMongoHandler(TextToSpeechReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateSpeechMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<TextToSpeechSessionReadModel>.Filter.Eq(x => x.Id, request.TextToSpeechId);

        await _readDbContext.TextToSpeechs.DeleteOneAsync(filter, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

