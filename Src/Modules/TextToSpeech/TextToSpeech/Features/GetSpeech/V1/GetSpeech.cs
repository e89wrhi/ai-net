using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using TextToSpeech.Data;
using TextToSpeech.Dtos;
using TextToSpeech.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace TextToSpeech.Features.GetTextToSpeechHistory.V1;

public record GetSpeech(Guid UserId) : IQuery<GetTextToSpeechHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetTextToSpeechHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetTextToSpeechHistoryResult(IEnumerable<TextToSpeechDto> TextToSpeechDtos);

public record GetTextToSpeechHistoryResponseDto(IEnumerable<TextToSpeechDto> TextToSpeechDtos);

public class GetTextToSpeechHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/texttospeech/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetSpeech(userId), cancellationToken);

                    var response = result.Adapt<GetTextToSpeechHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetTextToSpeechHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("TextToSpeech").Build())
            .Produces<GetTextToSpeechHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get TextToSpeech History")
            .WithDescription("Get TextToSpeech History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetTextToSpeechHistoryHandler : IQueryHandler<GetSpeech, GetTextToSpeechHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly TextToSpeechReadDbContext _readDbContext;

    public GetTextToSpeechHistoryHandler(IMapper mapper, TextToSpeechReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetTextToSpeechHistoryResult> Handle(GetSpeech request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var texttospeechs = await _readDbContext.TextToSpeechs.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var texttospeechDtos = _mapper.Map<IEnumerable<TextToSpeechDto>>(texttospeechs);

        return new GetTextToSpeechHistoryResult(texttospeechDtos);
    }
}
