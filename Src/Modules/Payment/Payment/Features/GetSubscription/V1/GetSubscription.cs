using AI.Common.Caching;
using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Payment.Data;
using Payment.Dtos;
using Payment.Exceptions;
using Duende.IdentityServer.EntityFramework.Entities;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Payment.Features.GetSubscription.V1;


public record GetSubscription : IQuery<GetSubscriptionResult>, ICacheRequest
{
    public string CacheKey => "GetSubscription";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetSubscriptionResult(IEnumerable<SubscriptionDto> SubscriptionDtos);

public record GetSubscriptionResponseDto(IEnumerable<SubscriptionDto> SubscriptionDtos);

public class GetSubscriptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetSubscription(), cancellationToken);

                    var response = result.Adapt<GetSubscriptionResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetSubscription")
            .WithApiVersionSet(builder.NewApiVersionSet("Subscription").Build())
            .Produces<GetSubscriptionResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Subscription")
            .WithDescription("Get Subscription")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetSubscriptionHandler : IQueryHandler<GetSubscription, GetSubscriptionResult>
{
    private readonly IMapper _mapper;
    private readonly PaymentReadDbContext _readDbContext;

    public GetSubscriptionHandler(IMapper mapper, PaymentReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetSubscriptionResult> Handle(GetSubscription request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var result = (await _readDbContext.Subscription.AsQueryable().ToListAsync(cancellationToken))
            .Where(i => i.Id == request.Id);

        if (!result.Any())
        {
            throw new SubscriptionNotFoundException(request.Id);
        }

        var eventDtos = _mapper.Map<IEnumerable<SubscriptionDto>>(result);

        return new GetSubscriptionResult(eventDtos);
    }
}
