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

namespace Resume.Features.GetResumeSuggestions.V1;


public record GetResumeSuggestions(Guid ResumeId) : IQuery<GetResumeSuggestionsResult>, ICacheRequest
{
    public string CacheKey => $"GetResumeSuggestions_{ResumeId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetResumeSuggestionsResult(IEnumerable<ResumeSuggestionDto> ResumeSuggestionDtos);

public record GetResumeSuggestionsResponseDto(IEnumerable<ResumeSuggestionDto> ResumeSuggestionDtos);

public class GetResumeSuggestionsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/resume/{{resumeId}}/suggestions",
                async (Guid resumeId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetResumeSuggestions(resumeId), cancellationToken);

                    var response = result.Adapt<GetResumeSuggestionsResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetResumeSuggestions")
            .WithApiVersionSet(builder.NewApiVersionSet("Resume").Build())
            .Produces<GetResumeSuggestionsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Resume Suggestions")
            .WithDescription("Get Resume Suggestions")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetResumeSuggestionsHandler : IQueryHandler<GetResumeSuggestions, GetResumeSuggestionsResult>
{
    private readonly IMapper _mapper;
    private readonly ResumeReadDbContext _readDbContext;

    public GetResumeSuggestionsHandler(IMapper mapper, ResumeReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetResumeSuggestionsResult> Handle(GetResumeSuggestions request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var resume = await _readDbContext.Resume.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == request.ResumeId, cancellationToken);

        if (resume == null)
        {
            throw new ResumeNotFoundException(request.ResumeId);
        }

        var dtos = _mapper.Map<IEnumerable<ResumeSuggestionDto>>(resume.Suggestions);

        return new GetResumeSuggestionsResult(dtos);
    }
}

