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


public record GetResumeAnalysis : IQuery<GetResumeAnalysisResult>, ICacheRequest
{
    public string CacheKey => "GetResumeAnalysis";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetResumeAnalysisResult(IEnumerable<ResumeAnalysisDto> ResumeDtos);

public record GetResumeAnalysisResponseDto(IEnumerable<ResumeAnalysisDto> ResumeDtos);

public class GetResumeAnalysisEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetResumeAnalysis(), cancellationToken);

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

        var result = (await _readDbContext.Resume.AsQueryable().ToListAsync(cancellationToken))
            .Where(i => i.Id == request.Id);

        if (!result.Any())
        {
            throw new ResumeNotFoundException(request.Id);
        }

        var eventDtos = _mapper.Map<IEnumerable<ResumeAnalysisDto>>(result);

        return new GetResumeAnalysisResult(eventDtos);
    }
}
