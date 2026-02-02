using System.Security.Claims;
using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Payment.Data;
using Payment.Dtos;
using Payment.Exceptions;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Payment.Features.GetSubscription.V1;

public record GetSubscription(Guid UserId) : IQuery<GetSubscriptionResult>, ICacheRequest
{
    public string CacheKey => $"GetSubscription_{UserId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetSubscriptionResult(SubscriptionDto SubscriptionDto);

public record GetSubscriptionResponseDto(SubscriptionDto SubscriptionDto);

public class GetSubscriptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/subscription",
                async (IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetSubscription(userId), cancellationToken);

                    var response = result.Adapt<GetSubscriptionResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetSubscription")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<GetSubscriptionResponseDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Subscription")
            .WithDescription("Gets the active subscription for the currently authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetSubscriptionHandler : IQueryHandler<GetSubscription, GetSubscriptionResult>
{
    private readonly IMapper _mapper;
    private readonly PaymentDbContext _dbContext;

    public GetSubscriptionHandler(IMapper mapper, PaymentDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetSubscriptionResult> Handle(GetSubscription request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _dbContext.Subscriptions
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Status == "Active", cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.UserId);
        }

        var dto = _mapper.Map<SubscriptionDto>(subscription);

        return new GetSubscriptionResult(dto);
    }
}

