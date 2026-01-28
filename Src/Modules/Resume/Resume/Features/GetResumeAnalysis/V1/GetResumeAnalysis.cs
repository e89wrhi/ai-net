using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Resume.Data;
using Resume.Dtos;
using Resume.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Resume.Features.GetResumeAnalysis.V1;


public record GetResumeAnalysis(Guid ResumeId) : IQuery<GetResumeAnalysisResult>, ICacheRequest
{
    public string CacheKey => $"GetResumeAnalysis_{ResumeId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetResumeAnalysisResult(ResumeAnalysisDto ResumeAnalysisDto);

public record GetResumeAnalysisResponseDto(ResumeAnalysisDto ResumeAnalysisDto);

public class GetResumeAnalysisEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/resume/{{resumeId}}/analysis",
                async (Guid resumeId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetResumeAnalysis(resumeId), cancellationToken);

                    var response = result.Adapt<GetResumeAnalysisResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetResumeAnalysis")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<GetResumeAnalysisResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Resume Analysis")
            .WithDescription("Get Resume Analysis")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetResumeAnalysisHandler : IQueryHandler<GetResumeAnalysis, GetResumeAnalysisResult>
{
    private readonly IMapper _mapper;
    private readonly ResumeReadDbContext _readDbContext;

    public GetResumeAnalysisHandler(IMapper mapper, ResumeReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetResumeAnalysisResult> Handle(GetResumeAnalysis request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var resume = await _readDbContext.Resume.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == request.ResumeId, cancellationToken);

        if (resume == null)
        {
            throw new ResumeNotFoundException(request.ResumeId);
        }

        var dto = _mapper.Map<ResumeAnalysisDto>(resume);

        return new GetResumeAnalysisResult(dto);
    }
}

