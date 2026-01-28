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


public record GetUserUsageSummary(Guid UserId) : IQuery<GetUserUsageSummaryResult>, ICacheRequest
{
    public string CacheKey => $"GetUserUsageSummary_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetUserUsageSummaryResult(IEnumerable<UserUsageSummaryDto> UserUsageSummaryDtos);

public record GetUserUsageSummaryResponseDto(IEnumerable<UserUsageSummaryDto> UserUsageSummaryDtos);

public class GetUserUsageSummaryEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/users/{{userId}}/usage-summary",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetUserUsageSummary(userId), cancellationToken);

                    var response = result.Adapt<GetUserUsageSummaryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetUserUsageSummary")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
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

        var user = await _readDbContext.User.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        var usageDtos = _mapper.Map<IEnumerable<UserUsageSummaryDto>>(user.Usages);

        return new GetUserUsageSummaryResult(usageDtos);
    }
}

