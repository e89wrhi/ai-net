using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using User.Data;
using User.Dtos;
using User.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace User.Features.GetUserUsageSummary.V1;


public record GetUserUsageSummary : IQuery<GetUserUsageSummaryResult>, ICacheRequest
{
    public string CacheKey => "GetUserUsageSummary";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetUserUsageSummaryResult(IEnumerable<UserUsageSummaryDto> UserUsageSummaryDtos);

public record GetUserUsageSummaryResponseDto(IEnumerable<UserUsageSummaryDto> UserUsageSummaryDtos);

public class GetUserUsageSummaryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetUserUsageSummary(), cancellationToken);

                    var response = result.Adapt<GetUserUsageSummaryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetUserUsageSummary")
            .WithApiVersionSet(builder.NewApiVersionSet("UserUsageSummary").Build())
            .Produces<GetUserUsageSummaryResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get User Usage Summary")
            .WithDescription("Get User Usage Summary")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetUserUsageSummaryHandler : IQueryHandler<GetUserUsageSummary, GetUserUsageSummaryResult>
{
    private readonly IMapper _mapper;
    private readonly UserReadDbContext _readDbContext;

    public GetUserUsageSummaryHandler(IMapper mapper, UserReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetUserUsageSummaryResult> Handle(GetUserUsageSummary request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var result = (await _readDbContext.User.AsQueryable().ToListAsync(cancellationToken))
            .Where(i => i.Id == request.Id);

        if (!result.Any())
        {
            throw new UserNotFoundException(request.Id);
        }

        var eventDtos = _mapper.Map<IEnumerable<UserUsageSummaryDto>>(result);

        return new GetUserUsageSummaryResult(eventDtos);
    }
}
