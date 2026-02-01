using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using SpeechToText.Data;
using SpeechToText.Dtos;
using SpeechToText.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace SpeechToText.Features.GetSpeechToTextHistory.V1;

public record GetText(Guid UserId) : IQuery<GetSpeechToTextHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetSpeechToTextHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetSpeechToTextHistoryResult(IEnumerable<SpeechToTextDto> SpeechToTextDtos);

public record GetSpeechToTextHistoryResponseDto(IEnumerable<SpeechToTextDto> SpeechToTextDtos);

public class GetSpeechToTextHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/speechtotext/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetText(userId), cancellationToken);

                    var response = result.Adapt<GetSpeechToTextHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetSpeechToTextHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("SpeechToText").Build())
            .Produces<GetSpeechToTextHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get SpeechToText History")
            .WithDescription("Get SpeechToText History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetSpeechToTextHistoryHandler : IQueryHandler<GetText, GetSpeechToTextHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly SpeechToTextReadDbContext _readDbContext;

    public GetSpeechToTextHistoryHandler(IMapper mapper, SpeechToTextReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetSpeechToTextHistoryResult> Handle(GetText request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var speechtotexts = await _readDbContext.SpeechToTexts.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var speechtotextDtos = _mapper.Map<IEnumerable<SpeechToTextDto>>(speechtotexts);

        return new GetSpeechToTextHistoryResult(speechtotextDtos);
    }
}
