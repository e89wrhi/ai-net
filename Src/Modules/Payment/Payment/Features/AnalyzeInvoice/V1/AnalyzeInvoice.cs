using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Payment.Features.AnalyzeInvoice.V1;

public class AnalyzeInvoiceEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/payment/analyze-invoice",
                async (AnalyzeInvoiceWithAIRequestDto request, IMediator mediator, IHttpContextAccessor httpContextAccessor, CancellationToken cancellationToken) =>
                {
                    // current user id
                    var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new AnalyzeInvoiceWithAICommand(request.InvoiceId);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeInvoiceWithAIResponseDto(result.Summary, result.Analysis, result.HasAnomalies));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeInvoice")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<AnalyzeInvoiceWithAIResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Analyze Invoice with AI")
            .WithDescription("Uses AI to summarize invoice details and detect any unusual spending patterns.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
