using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using Meeting.Data;
using Meeting.Models;
using Meeting.ValueObjects;
using Meeting.Enums;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace Meeting.Features.ExtractActionItems.V1;

public class ExtractActionItemsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/meeting/action-items",
                async (ExtractActionItemsRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new ExtractActionItemsCommand(request.Transcript);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new ExtractActionItemsResponseDto(result.MeetingId, result.ActionItems));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("ExtractActionItems")
            .WithApiVersionSet(builder.NewApiVersionSet("Meeting").Build())
            .Produces<ExtractActionItemsResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Extract Meeting Action Items")
            .WithDescription("Uses AI to extract only the actionable tasks and assignments from a meeting transcript.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
