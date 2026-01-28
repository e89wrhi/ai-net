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

public record ReAnalyzeResumeMongo() : InternalCommand;

public class ReAnalyzeResumeMongoHandler : ICommandHandler<ReAnalyzeResumeMongo>
{
    private readonly ResumeReadDbContext _readDbContext;
    private readonly IMapper _mapper;

    public ReAnalyzeResumeMongoHandler(
        ResumeReadDbContext readDbContext,
        IMapper mapper)
    {
        _readDbContext = readDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(ReAnalyzeResumeMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var eventReadModel = _mapper.Map<ResumeReadModel>(request);


        return Unit.Value;
    }
}
