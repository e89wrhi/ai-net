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

namespace ImageEdit.Features.AIEnhanceImage.V1;

public class AIEnhanceImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imageedit/enhance",
                async (AIEnhanceImageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AIEnhanceImageCommand(request.ImageUrlOrBase64, request.Prompt);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AIEnhanceImageResponseDto(result.SessionId, result.ResultId, result.ResultImageUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AIEnhanceImage")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageEdit").Build())
            .Produces<AIEnhanceImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Enhance Image with AI")
            .WithDescription("Uses AI to enhance the quality or details of an image based on a prompt.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
