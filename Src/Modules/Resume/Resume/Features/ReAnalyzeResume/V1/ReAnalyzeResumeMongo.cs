namespace Resume.Features.ReAnalyzeResume.V1;


using Ardalis.GuardClauses;
using Resume.Data;
using Resume.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AI.Common.Core;
using Resume.Exceptions;
using System;

public record ReAnalyzeResumeMongo(Guid ResumeId, string Summary, string ParsedText, List<string> Skills, List<string> Suggestions, string Status, DateTime AnalyzedAt) : InternalCommand;

public class ReAnalyzeResumeMongoHandler : ICommandHandler<ReAnalyzeResumeMongo>
{
    private readonly ResumeReadDbContext _readDbContext;

    public ReAnalyzeResumeMongoHandler(ResumeReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Unit> Handle(ReAnalyzeResumeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var filter = Builders<ResumeReadModel>.Filter.Eq(x => x.Id, request.ResumeId);
        
        var update = Builders<ResumeReadModel>.Update
            .Set(x => x.Summary, request.Summary)
            .Set(x => x.ParsedText, request.ParsedText)
            .Set(x => x.Skills, request.Skills)
            .Set(x => x.Suggestions, request.Suggestions)
            .Set(x => x.Status, request.Status)
            .Set(x => x.AnalyzedAt, request.AnalyzedAt);

        await _readDbContext.Resume.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}

