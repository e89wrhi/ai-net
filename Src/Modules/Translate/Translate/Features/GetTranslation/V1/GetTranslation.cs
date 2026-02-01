using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Translate.Data;
using Translate.Dtos;
using Translate.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Translate.Features.GetTranslateHistory.V1;

public record GetTranslation(Guid UserId) : IQuery<GetTranslateHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetTranslateHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetTranslateHistoryResult(IEnumerable<TranslateDto> TranslateDtos);

public record GetTranslateHistoryResponseDto(IEnumerable<TranslateDto> TranslateDtos);

public class GetTranslateHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/translate/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetTranslation(userId), cancellationToken);

                    var response = result.Adapt<GetTranslateHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetTranslateHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("Translate").Build())
            .Produces<GetTranslateHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Translate History")
            .WithDescription("Get Translate History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetTranslateHistoryHandler : IQueryHandler<GetTranslation, GetTranslateHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly TranslateReadDbContext _readDbContext;

    public GetTranslateHistoryHandler(IMapper mapper, TranslateReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetTranslateHistoryResult> Handle(GetTranslation request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var translates = await _readDbContext.Translates.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var translateDtos = _mapper.Map<IEnumerable<TranslateDto>>(translates);

        return new GetTranslateHistoryResult(translateDtos);
    }
}
