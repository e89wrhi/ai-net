using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageCaption.Data;
using ImageCaption.Models;
using ImageCaption.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ImageCaption.Features.AIImageCaption.V1;

public class AIImageCaptionEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/ai-caption",
                async (AIImageCaptionRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AIImageCaptionCommand(request.ImageUrlOrBase64);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AIImageCaptionResponseDto(result.SessionId, result.ResultId, result.Caption));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AIImageCaption")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<AIImageCaptionResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Image Caption with AI")
            .WithDescription("Uses AI to generate a descriptive caption for the provided image.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
