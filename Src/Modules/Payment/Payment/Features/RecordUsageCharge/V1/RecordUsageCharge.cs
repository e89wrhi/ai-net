using AI.Common.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Payment.Features.RecordUsageCharge.V1;


public class RecordUsageChargeEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/subscription/charge/record", async (RecordUsageChargeRequest request,
                IMediator mediator, IMapper mapper,
                CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<RecordUsageChargeCommand>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<RecordUsageChargeRequestResponse>();

            return Results.Ok(response);
        })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("RecordUsageCharge")
            .WithApiVersionSet(builder.NewApiVersionSet("Payment").Build())
            .Produces<RecordUsageChargeRequestResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest).ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Record Usage Charge")
            .WithDescription("Record Usage Charge")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
