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
        builder.MapGet($"{EndpointConfig.BaseApiPath}/users/usage-summary",
                async (IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetUserUsageSummary(userId), cancellationToken);

                    var response = result.Adapt<GetUserUsageSummaryResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetUserUsageSummary")
            .WithApiVersionSet(builder.NewApiVersionSet("User").Build())
            .Produces<GetUserUsageSummaryResponseDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get User Usage Summary")
            .WithDescription("Gets the usage statistics for the currently authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetUserUsageSummaryHandler : IQueryHandler<GetUserUsageSummary, GetUserUsageSummaryResult>
{
    private readonly IMapper _mapper;
    private readonly UserDbContext _dbContext;

    public GetUserUsageSummaryHandler(IMapper mapper, UserDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetUserUsageSummaryResult> Handle(GetUserUsageSummary request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var analytics = await _dbContext.UserAnalytics
            .FirstOrDefaultAsync(x => x.User.Value == request.UserId, cancellationToken);

        if (analytics == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        // Map from SQL analytics model to DTOs
        // Here we create a summary based on the multiple request counters in the analytics model
        var usageDtos = new List<UserUsageSummaryDto>
        {
            new() { Id = Guid.NewGuid(), Period = "Today", RequestsCount = (int)analytics.TodayRequests, TokenUsed = "N/A" },
            new() { Id = Guid.NewGuid(), Period = "This Week", RequestsCount = (int)analytics.WeekRequests, TokenUsed = "N/A" },
            new() { Id = Guid.NewGuid(), Period = "This Month", RequestsCount = (int)analytics.MonthRequests, TokenUsed = "N/A" },
            new() { Id = Guid.NewGuid(), Period = "Total", RequestsCount = (int)analytics.TotalRequests, TokenUsed = "N/A" }
        };

        return new GetUserUsageSummaryResult(usageDtos);
    }
}

