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

namespace User.Features.GetUserActivity.V1;


public record GetUserActivity(Guid UserId) : IQuery<GetUserActivityResult>, ICacheRequest
{
    public string CacheKey => $"GetUserActivity_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetUserActivityResult(IEnumerable<UserActivityDto> UserActivityDtos);

public record GetUserActivityResponseDto(IEnumerable<UserActivityDto> UserActivityDtos);

public class GetUserActivityEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/users/{{userId}}/activities",
                async (Guid userId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetUserActivity(userId), cancellationToken);

                    var response = result.Adapt<GetUserActivityResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetUserActivity")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<GetUserActivityResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get User Activity")
            .WithDescription("Get User Activity")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetUserActivityHandler : IQueryHandler<GetUserActivity, GetUserActivityResult>
{
    private readonly IMapper _mapper;
    private readonly UserReadDbContext _readDbContext;

    public GetUserActivityHandler(IMapper mapper, UserReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetUserActivityResult> Handle(GetUserActivity request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await _readDbContext.User.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        var activityDtos = _mapper.Map<IEnumerable<UserActivityDto>>(user.Activities);

        return new GetUserActivityResult(activityDtos);
    }
}

