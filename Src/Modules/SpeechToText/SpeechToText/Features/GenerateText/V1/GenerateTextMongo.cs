namespace SpeechToText.Features.StartSpeechToText.V1;


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

public record GenerateTextMongo(Guid Id, Guid UserId, string Title, string AiModelId, string SessionStatus, DateTime CreatedAt) : InternalCommand;

public class StartSpeechToTextMongoHandler : ICommandHandler<GenerateTextMongo>
{
    private readonly SpeechToTextReadDbContext _readDbContext;

    public StartSpeechToTextMongoHandler(SpeechToTextReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GenerateTextMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var speechtotext = new SpeechToTextReadModel
        {
            Id = request.Id,
            UserId = request.UserId,
            Title = request.Title,
            AiModelId = request.AiModelId,
            SessionStatus = request.SessionStatus,
            LastSentAt = request.CreatedAt,
            SpeechToTexts = new List<SpeechToTextReadModel>()
        };

        await _readDbContext.SpeechToTexts.InsertOneAsync(speechtotext, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

