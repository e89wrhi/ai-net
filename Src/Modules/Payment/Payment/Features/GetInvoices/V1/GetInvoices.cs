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

namespace Payment.Features.GetInvoices.V1;

public record GetInvoices(Guid SubscriptionId, Guid UserId) : IQuery<GetInvoicesResult>, ICacheRequest
{
    public string CacheKey => $"GetInvoices_{SubscriptionId}";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

public record GetInvoicesResult(IEnumerable<InvoiceDto> InvoiceDtos);

public record GetInvoicesResponseDto(IEnumerable<InvoiceDto> InvoiceDtos);

public class GetInvoicesEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/subscription/{{subscriptionId}}/invoices",
                async (Guid subscriptionId, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await mediator.Send(new GetInvoices(subscriptionId, userId), cancellationToken);

                    var response = result.Adapt<GetInvoicesResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetInvoices")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<GetInvoicesResponseDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Invoices")
            .WithDescription("Gets the invoices for a specific subscription, ensuring it belongs to the authenticated user.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetInvoicesHandler : IQueryHandler<GetInvoices, GetInvoicesResult>
{
    private readonly IMapper _mapper;
    private readonly PaymentDbContext _dbContext;

    public GetInvoicesHandler(IMapper mapper, PaymentDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<GetInvoicesResult> Handle(GetInvoices request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _dbContext.Subscriptions
            .Include(x => x.Invoices)
            .FirstOrDefaultAsync(x => x.Id == request.SubscriptionId && x.UserId == request.UserId, cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.SubscriptionId);
        }

        var dtos = _mapper.Map<IEnumerable<InvoiceDto>>(subscription.Invoices);

        return new GetInvoicesResult(dtos);
    }
}

