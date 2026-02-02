using System.Security.Claims;
using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using User.Data;
using User.Dtos;
using User.Exceptions;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

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
        builder.MapGet($"{EndpointConfig.BaseApiPath}/users/activities",
                async (IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetUserActivity(userId), cancellationToken);

                    var response = result.Adapt<GetUserActivityResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetUserActivity")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<GetUserActivityResponseDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get User Activity")
            .WithDescription("Gets the activity history for the currently authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetUserActivityHandler : IQueryHandler<GetUserActivity, GetUserActivityResult>
{
    private readonly IMapper _mapper;
    private readonly UserDbContext _dbContext;

    public GetUserActivityHandler(IMapper mapper, UserDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetUserActivityResult> Handle(GetUserActivity request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var activities = await _dbContext.Sessions
            .Include(x => x.Actions)
            .Where(x => x.UserId.Value == request.UserId)
            .OrderByDescending(x => x.LastActivityAt)
            .ToListAsync(cancellationToken);

        // Flatten sessions into activity DTOs
        var activityDtos = activities.SelectMany(s => s.Actions.Select(a => new UserActivityDto
        {
            Id = a.Id,
            Module = "Internal", // This could be mapped from session if available
            Action = a.ActionType,
            TimeStamp = a.OccurredAt,
            ResourceId = s.Id
        })).ToList();

        return new GetUserActivityResult(activityDtos);
    }
}

