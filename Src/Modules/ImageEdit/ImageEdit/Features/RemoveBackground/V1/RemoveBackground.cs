using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageEdit.Data;
using ImageEdit.Enums;
using ImageEdit.Models;
using ImageEdit.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ImageEdit.Features.RemoveBackground.V1;

public class RemoveBackgroundEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imageedit/remove-background",
                async (RemoveBackgroundRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new RemoveBackgroundCommand(request.ImageUrlOrBase64);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new RemoveBackgroundResponseDto(result.SessionId, result.ResultId, result.ResultImageUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("RemoveBackground")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<RemoveBackgroundResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Remove Background with AI")
            .WithDescription("Uses AI to automatically detect and remove the background from an image.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
