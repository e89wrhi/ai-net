namespace Summary.Features.StartSummary.V1;


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

public record GetSummaryMongo(Guid Id, Guid UserId, string Title, string AiModelId, string SessionStatus, DateTime CreatedAt) : InternalCommand;

public class StartSummaryMongoHandler : ICommandHandler<GetSummaryMongo>
{
    private readonly SummaryReadDbContext _readDbContext;

    public StartSummaryMongoHandler(SummaryReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(GetSummaryMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var summary = new SummaryReadModel
        {
            Id = request.Id,
            UserId = request.UserId,
            Title = request.Title,
            AiModelId = request.AiModelId,
            SessionStatus = request.SessionStatus,
            LastSentAt = request.CreatedAt,
            Summarys = new List<SummaryReadModel>()
        };

        await _readDbContext.Summarys.InsertOneAsync(summary, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

