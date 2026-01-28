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

namespace Payment.Features.GetInvoices.V1;


public record GetInvoices(Guid SubscriptionId) : IQuery<GetInvoicesResult>, ICacheRequest
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
                async (Guid subscriptionId, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetInvoices(subscriptionId), cancellationToken);

                    var response = result.Adapt<GetInvoicesResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetInvoices")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<GetInvoicesResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Invoices")
            .WithDescription("Get Invoices")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

internal class GetInvoicesHandler : IQueryHandler<GetInvoices, GetInvoicesResult>
{
    private readonly IMapper _mapper;
    private readonly PaymentReadDbContext _readDbContext;

    public GetInvoicesHandler(IMapper mapper, PaymentReadDbContext readDbContext)
    {
        _mapper = mapper;
        _readDbContext = readDbContext;
    }

    public async Task<GetInvoicesResult> Handle(GetInvoices request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var subscription = await _readDbContext.Subscription.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == request.SubscriptionId, cancellationToken);

        if (subscription == null)
        {
            throw new SubscriptionNotFoundException(request.SubscriptionId);
        }

        var dtos = _mapper.Map<IEnumerable<InvoiceDto>>(subscription.Invoices);

        return new GetInvoicesResult(dtos);
    }
}

