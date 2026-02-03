using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Payment.Features.GenerateInvoice.V1;


public class GenerateInvoiceEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/subscription/invoice/generate", async (GenerateInvoiceRequest request,
                IMediator mediator, IHttpContextAccessor httpContextAccessor, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            // current user id
            var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = mapper.Map<GenerateInvoiceCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<GenerateInvoiceRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateInvoice")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<GenerateInvoiceRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Invoice")
            .WithDescription("Generate Invoice")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

