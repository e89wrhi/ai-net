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

namespace ImageCaption.Features.AnalyzeImage.V1;

public class AnalyzeImageEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/image/analyze",
                async (AnalyzeImageRequestDto request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var command = new AnalyzeImageCommand(request.ImageUrlOrBase64);
                    var result = await mediator.Send(command, cancellationToken);
                    return Results.Ok(new AnalyzeImageResponseDto(result.SessionId, result.ResultId, result.Analysis));
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("AnalyzeImage")
            .WithApiVersionSet(builder.NewApiVersionSet("Image").Build())
            .Produces<AnalyzeImageResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Analyze Image with AI")
            .WithDescription("Uses AI to provide a detailed analysis of the provided image, including objects, colors, and context.")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}
