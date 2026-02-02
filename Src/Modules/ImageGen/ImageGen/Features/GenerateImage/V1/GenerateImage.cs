using AI.Common.Core;
using AI.Common.Web;
using Ardalis.GuardClauses;
using ImageGen.Data;
using ImageGen.Enums;
using ImageGen.Models;
using ImageGen.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.AI;

namespace ImageGen.Features.GenerateImage.V1;

public class GenerateImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/imagegen/generate",
                async (GenerateImageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new GenerateImageCommand(request.Prompt, request.Size, request.Style);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new GenerateImageResponseDto(result.SessionId, result.ResultId, result.ImageUrl));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GenerateImage")
            .WithApiVersionSet(builder.NewApiVersionSet("ImageGen").Build())
            .Produces<GenerateImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Generate Image")
            .WithDescription("Generates an image from a text prompt using AI.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
