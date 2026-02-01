using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using AutoComplete.Data;
using AutoComplete.Dtos;
using AutoComplete.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AutoComplete.Features.GetAutoCompleteHistory.V1;

public record GetAutocomplete(Guid UserId) : IQuery<GetAutoCompleteHistoryResult>, ICacheRequest
{
    public string CacheKey => $"GetAutoCompleteHistory_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetAutoCompleteHistoryResult(IEnumerable<AutocompleteDto> AutoCompleteDtos);

public record GetAutoCompleteHistoryResponseDto(IEnumerable<AutocompleteDto> AutoCompleteDtos);

public class GetAutoCompleteHistoryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/autoComplete/history/{{userId}}",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetAutocomplete(userId), cancellationToken);

                    var response = result.Adapt<GetAutoCompleteHistoryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetAutoCompleteHistory")
            .WithApiVersionSet(builder.NewApiVersionSet("AutoComplete").Build())
            .Produces<GetAutoCompleteHistoryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get AutoComplete History")
            .WithDescription("Get AutoComplete History")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetAutoCompleteHistoryHandler : IQueryHandler<GetAutocomplete, GetAutoCompleteHistoryResult>
{
    private readonly IMapper _mapper;
    private readonly AutocompleteReadDbContext _readDbContext;

    public GetAutoCompleteHistoryHandler(IMapper mapper, AutocompleteReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetAutoCompleteHistoryResult> Handle(GetAutocomplete request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var autoCompletes = await _readDbContext.AutoCompletes.AsQueryable()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var autoCompleteDtos = _mapper.Map<IEnumerable<AutocompleteDto>>(autoCompletes);

        return new GetAutoCompleteHistoryResult(autoCompleteDtos);
    }
}
