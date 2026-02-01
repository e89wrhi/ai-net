using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Sentiment.Data;
using Sentiment.Dtos;
using Sentiment.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Sentiment.Features.GetSentimentHistory.V1;

public record GetSentiment(Guid UserId) : IQuery<GetSentimentHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetSentimentHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetSentimentHistoryResult(IEnumerable<SentimentDto> SentimentDtos);

public record GetSentimentHistoryResponseDto(IEnumerable<SentimentDto> SentimentDtos);

public class GetSentimentHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/sentiment/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetSentiment(userId), cancellationToken);

                    var response = result.Adapt<GetSentimentHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetSentimentHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("Sentiment").Build())
            .Produces<GetSentimentHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Sentiment History")
            .WithDescription("Get Sentiment History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetSentimentHistoryHandler : IQueryHandler<GetSentiment, GetSentimentHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly SentimentReadDbContext _readDbContext;

    public GetSentimentHistoryHandler(IMapper mapper, SentimentReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetSentimentHistoryResult> Handle(GetSentiment request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var sentiments = await _readDbContext.Sentiments.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var sentimentDtos = _mapper.Map<IEnumerable<SentimentDto>>(sentiments);

        return new GetSentimentHistoryResult(sentimentDtos);
    }
}
